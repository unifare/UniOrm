using System;
using System.Collections.Generic;
using System.Reflection;  
using PetaPoco;
using System.Linq; 
using System.Linq.Expressions;
using UniOrm.Loggers;

namespace UniOrm.Adaption
{
    public class PatePocoOrmAdaptor : OrmAdaptorBase, IOrmAdaptor
    {

        DBType? _dBType;
        private readonly string LoggerName= "PatePocoOrmAdaptor";

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
        //public string ConnectionString { get; set; }

        public override OrmType OrmType
        {
            get
            { 
                return OrmType.Sql;
            }
        }


        public PatePocoOrmAdaptor()
        {
            OrmName = OrmName.PatePoco;
            //Name = "PatePoco";
        }

        public List<Assembly> GetAssemblies()
        {
            return new List<Assembly>() { typeof(PetaPoco.Database).Assembly };
        }

        public void GetCloseAction(object dbOperator)
        {
            // var db = dbOperator as Database; 
        }
        public int GetSqlCommandAction(object dbOperator, string sql, params object[] objparameters)
        {
            if (objparameters == null)
            {
                return 0;
            }
            var db = dbOperator as Database;
            var reint = 0;
            reint = db.Execute(sql, objparameters);

            return reint;
        }
        public object GetSqlExecuteScalarAction(object dbOperator, string sql, params object[] objparameters)
        {
            var db = dbOperator as Database;
            var reint = db.ExecuteScalar<dynamic>(sql, objparameters);

            return reint;
        }



        public object GetDeleteAction(object dbOperator, params object[] objparameters)
        {
            if (objparameters == null)
            {
                return 0;
            }
            var db = dbOperator as Database;
            var reint = 0;
            foreach (var o in objparameters)
            {
                reint += db.Delete(o);
            }
            return reint;
        }
        public bool GetBeginTransicationAction()
        {
            return false;
        }
        public object GetCeateAction(params object[] objparameters)
        {
            var connectionstring = objparameters[0] as string;
            var provider = objparameters[1] as string;
            var db = new Database(connectionstring, provider);
            //db.Insertable(new SqlBuilderAccessory()).ExecuteCommandAsync
            return db;
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
            var db = dbOperator as Database;
            object reint = null;
            foreach (var obj in objparameters)
            {
                reint = db.Insert(obj);
            }
            return reint;
        }

        //public object GetOpenSessionAction(object dbOperator, params object[] objparameters)
        //{
        //    return dbOperator;
        //}
        public List<dynamic> GetQuertyAction(object dbOperator, string sql, params object[] args)
        {

            var db = dbOperator as Database;
            var query = db.Query<dynamic>(sql, args).ToList();

            return query;
        }

        public QueryResult GetQueryPageAction(object dbOperator, string sql, int startpage, int pagesize, params object[] args)
        {
            if (pagesize <= 0)
            {
                pagesize = 100;
            }
            var db = dbOperator as Database;
            var query = db.SkipTake<dynamic>(startpage * pagesize, pagesize, sql, args);
            var alldatacount = db.ExecuteScalar<int>("seletct count(1) from (" + sql + ")", args);
            var relist = new QueryResult()
            {
                DataList = query,
                currentIndex = startpage + 1,
                PageSize = pagesize,
                TotalPage = alldatacount
            };
            return relist;
        }


        public int GetUpdatection(object dbOperator, params object[] objparameters)
        {
            var db = dbOperator as Database;
            var reint = 0;
            foreach (var o in objparameters)
            {
                reint = db.Update(o);
            }
            return reint;
        }

        public IEnumerable<T> GetSqlQueryAction<T>(object dbOperator, string sql, params object[] args) where T : class, new()
        {
            var db = dbOperator as Database;
            var query = db.Query<T>(sql, args).ToList();

            return query;
        }

        public IEnumerable<TSource> QueryList<TSource>(object dbOperator, Expression<Func<TSource, bool>> predicate) where TSource : class, new()
        {
            //var db = dbOperator as Database;
            ///var query = db.Query<TSource>(sql, args).ToList();
            Logger.LogError(LoggerName, "QueryList<TSource> -> not implement for PatePoco ");
            return null;
        }
    }
}
