using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text; 
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using UniOrm.Common;
using System.Linq.Expressions;

namespace UniOrm.Adaption
{
    public class EFCoreDataAdaper : OrmAdaptorBase, IOrmAdaptor
    {
        public static MemoryCache memoryCache = null;
        private readonly static object Singleton_Lock = new object(); //锁同步
        DBType? _dBType;
        public override DBType dBType
        {
            get
            {
                if (_dBType == null)
                {
                    _dBType = (DBType)ConnectionConfig.DBType;
                }
                return _dBType.Value;
            }
        }
        public override OrmType OrmType
        {
            get
            {
                return OrmType.Sql;
            }
        }



        public EFCoreDataAdaper()
        {
            DataGrounderType = typeof(ITypeDataGroud);
            OrmName =  OrmName.EFCore;
        }
        public object GetCeateAction(params object[] objparameters)
        {

            if (IsSelfDefineCreation)
            {
                return SelfGetCeateAction(objparameters);
            }
            else
            {
                var connectionstring = objparameters[0] as string;
                var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();

                switch (dBType)
                {
                    case DBType.Mysql:

                        optionsBuilder.UseMySql(connectionstring);
                        break;
                    case DBType.Sqlite:

                        optionsBuilder.UseSqlite(connectionstring);
                        break;
                    case DBType.InMemory:

                        optionsBuilder.UseInMemoryDatabase(connectionstring);
                        break;
                    case DBType.SqlServer:

                        optionsBuilder.UseSqlServer(connectionstring,
                             x => x.MigrationsHistoryTable("__MyMigrationsHistory", "VersionInfo").UseRowNumberForPaging()
                            ); 
                        break;
                    case DBType.Postgre:

                        optionsBuilder.UseNpgsql(connectionstring);
                        break;
                }
              
                lock (Singleton_Lock)
                {
                    memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
                }
                var db = new EFDbContext(optionsBuilder.Options, memoryCache, RegistedModelTypes.GetAllTypes());
                db.DefaultDbPrefixName = ConnectionConfig. DefaultDbPrefixName;
                return db;
            }
        }
        public List<Assembly> GetAssemblies()
        {
            return new List<Assembly>() { typeof(PetaPoco.Database).Assembly };
        }


        public int GetSqlCommandAction(object dbOperator, string sql, params object[] objparameters)
        {
            var db = dbOperator as EFDbContext;
            var reint = db.Database.ExecuteSqlCommand(sql, objparameters);
            return reint;
        }
        public object GetSqlExecuteScalarAction(object dbOperator, string sql, params object[] objparameters)
        {

            return ExecuteScalarAsync(dbOperator, sql, objparameters);
        }

        public object ExecuteScalarAsync(object dbOperator, RawSqlString sql, params object[] parameters)
        {
            var db = dbOperator as EFDbContext;
            using (db.GetService<IConcurrencyDetector>().EnterCriticalSection())
            {
                RawSqlCommand rawSqlCommand = db.GetService<IRawSqlCommandBuilder>().Build(sql.Format, parameters);
                return rawSqlCommand.RelationalCommand.ExecuteScalar(db.GetService<IRelationalConnection>(), rawSqlCommand.ParameterValues);
            }
        }

        public object GetDeleteAction(object dbOperator, params object[] objparameters)
        {
            if (objparameters == null)
            {
                return 0;
            }
            var db = dbOperator as EFDbContext;
            var reint = 0;
            foreach (var o in objparameters)
            {
                db.Remove(o);
                reint++;
            }
            db.SaveChanges();
            return reint;
        }
        public bool GetBeginTransicationAction()
        {
            return false;
        }

        public object[] GetCeateActionParamters()
        {
            var connectionstring = this.ConnectionConfig.Connectionstring;
            var provider = "sqlite";

            switch (dBType)
            {
                case DBType.Mysql:
                    provider = "msssql";
                    break;
                case DBType.Sqlite:
                    provider = "sqlite";
                    break;
                case DBType.SqlServer:
                    provider = "SqlServer";
                    break;
            }
            //db.Insertable(new SqlBuilderAccessory()).ExecuteCommandAsync
            return new object[] { connectionstring, provider };
        }

        public object GetInsertAction(object dbOperator, params object[] objparameters)
        {
            var db = dbOperator as EFDbContext;
            var reint = 0;

            foreach (var obj in objparameters)
            {
                db.Add(obj);//.FirstOrDefault(p => p.TableName == obj.GetType().ReflectedType.Name);
                reint++;

            }
            db.SaveChanges();
            return reint;
        }

        //public object GetOpenSessionAction(object dbOperator, params object[] objparameters)
        //{
        //    return dbOperator;
        //}
        public List<dynamic> GetQuertyAction(object dbOperator, string sql, params object[] args)
        {
            return GetQuertyActionlocal(dbOperator, sql, args).ToList();
        }
        private IEnumerable<dynamic> GetQuertyActionlocal(object dbOperator, RawSqlString sql, params object[] args)
        {

            var db = dbOperator as EFDbContext;
            using (db.GetService<IConcurrencyDetector>().EnterCriticalSection())
            {
                IRelationalCommand SqlCommand = null;
                if (args == null)
                {
                    SqlCommand = db.GetService<IRawSqlCommandBuilder>().Build(sql.Format);
                    using (var dataReader = SqlCommand.ExecuteReader(db.GetService<IRelationalConnection>()))
                    {
                        while (dataReader.Read())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            for (var fieldCount = 0; fieldCount < dataReader.DbDataReader.FieldCount; fieldCount++)
                            {
                                row.Add(dataReader.DbDataReader.GetName(fieldCount), dataReader.DbDataReader[fieldCount]);
                            }
                            yield return row;
                        }
                    }
                }
                else
                {
                    var rawSqlCommand = db.GetService<IRawSqlCommandBuilder>().Build(sql.Format, args);
                    using (var dataReader = rawSqlCommand.RelationalCommand.ExecuteReader(db.GetService<IRelationalConnection>(), rawSqlCommand.ParameterValues))
                    {
                        while (dataReader.Read())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            for (var fieldCount = 0; fieldCount < dataReader.DbDataReader.FieldCount; fieldCount++)
                            {
                                row.Add(dataReader.DbDataReader.GetName(fieldCount), dataReader.DbDataReader[fieldCount]);
                            }
                            yield return row;
                        }
                    }
                }


            }
        }

        public QueryResult GetQueryPageAction(object dbOperator, string sql, int startpage, int pagesize, params object[] args)
        {
            if (pagesize <= 0)
            {
                pagesize = 100;
            }
            // var query = db.SkipTake<dynamic>(startpage * pagesize, pagesize, sql, args);
            var alldata = GetQuertyActionlocal(dbOperator, sql, args);
            var tagelist = alldata.Skip(startpage * pagesize).Take(pagesize);

            var relist = new QueryResult()
            {
                DataList = tagelist.ToList(),
                currentIndex = startpage + 1,
                PageSize = pagesize,
                TotalPage = alldata.Count()
            };
            return relist;
        }


        public int GetUpdatection(object dbOperator, params object[] objparameters)
        {
            var db = dbOperator as EFDbContext;
            var reint = 0;
            foreach (var o in objparameters)
            {
                var table = db.Update(o); //.FirstOrDefault(p => p.TableName == o.GetType().ReflectedType.Name);
                //if (db.Entry<T>(model).State == EntityState.Detached)
                //{
                //    try
                //    {
                //        db.Set<T>().Attach(model);
                //        db.Entry<T>(model).State = EntityState.Modified;
                //    }
                //    catch (InvalidOperationException)
                //    {
                //        T old = Find(model._ID);
                //        db.Entry(old).CurrentValues.SetValues(model);
                //    }
                //    db.SaveChanges();
                //}
            }
            db.SaveChanges();
            return reint;
        }
        public IEnumerable<T> GetSqlQueryAction<T>(object dbOperator, string sql, params object[] parameters) where T : class, new()
        {
            return GetSqlQueryActionLocal<T>(dbOperator, sql, parameters);
        }
        public IEnumerable<T> GetSqlQueryActionLocal<T>(object dbOperator, RawSqlString sql, params object[] parameters) where T : class, new()
        {
            var db = dbOperator as EFDbContext;
            return db.Set<T>().FromSql(sql, parameters);
            //var q = db.Set<T>().FromSql(sql, parameters);
            //var sewet = q.ToList<T>();
            //var propts = typeof(T).GetProperties();
            //using (db.GetService<IConcurrencyDetector>().EnterCriticalSection())
            //{
            //    RawSqlCommand rawSqlCommand = db.GetService<IRawSqlCommandBuilder>().Build(sql.Format, parameters);

            //    using (var dataReader = rawSqlCommand.RelationalCommand.ExecuteReader(db.GetService<IRelationalConnection>(), rawSqlCommand.ParameterValues))
            //    {
            //        while (dataReader.Read())
            //        {
            //            var model = new T();
            //            foreach (var l in propts)
            //            {
            //                var val = dataReader.DbDataReader[l.Name];
            //                if (val == DBNull.Value)
            //                {
            //                    l.SetValue(model, null);
            //                }
            //                else
            //                {
            //                    l.SetValue(model, val);
            //                }
            //            }

            //            yield return model;
            //        }
            //    }

            //}


            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            //var conn = db.Database.GetDbConnection();
            //try
            //{
            //    conn.Open();
            //    using (var command = conn.CreateCommand())
            //    {
            //        command.CommandText = sql;
            //        foreach(var op in parameters)
            //        {
            //            //db.par
            //        }
            //        command.Parameters.AddRange(parameters);
            //        var propts = typeof(T).GetProperties();
            //        var rtnList = new List<T>();
            //        T model;
            //        object val;
            //        using (var reader = command.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                model = new T();
            //                foreach (var l in propts)
            //                {
            //                    val = reader[l.Name];
            //                    if (val == DBNull.Value)
            //                    {
            //                        l.SetValue(model, null);
            //                    }
            //                    else
            //                    {
            //                        l.SetValue(model, val);
            //                    }
            //                }
            //                rtnList.Add(model);
            //            }
            //        }
            //        return rtnList;
            //    }
            //}
            //finally
            //{
            //    conn.Close();
            //}

            //var query = db.Query<dynamic>(sql, args).ToList();

        }

        public IEnumerable<TSource> QueryList<TSource>(object dbOperator, Expression<Func<TSource, bool>> predicate) where TSource : class, new() 
        {
            var db = dbOperator as EFDbContext;
            return db.Set<TSource>().Where(predicate);
        }
    }
}
