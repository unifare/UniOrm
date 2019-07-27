using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using UniOrm.ReflectionMagic;  
using System.Linq.Expressions;

namespace UniOrm.Adaption
{
    public class MemoryOrmADaptor : OrmAdaptorBase, IOrmAdaptor
    {

        public override OrmType OrmType
        {
            get
            {
                return OrmType.Type;
            }
        }
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
        //public string ConnectionString { get; set; }
        public MemoryOrmADaptor()
        {
            OrmName = OrmName.InMemory;
            //Name = "InMemory";
        }


        public List<Assembly> GetAssemblies()
        {
            return new List<Assembly>() { typeof(PetaPoco.Database).Assembly };
        }


        public int GetSqlCommandAction(object dbOperator, string sql, params object[] objparameters)
        {
            //if (objparameters == null)
            //{
            //    return 0;
            //}
            //var db = dbOperator as MemoryOrm     ;
            var reint = 0;
            //reint = db.Execute(sql, objparameters);

            return reint;
        }
        public object GetSqlExecuteScalarAction(object dbOperator, string sql, params object[] objparameters)
        {
            //    var db = dbOperator as Database;
            //    var reint = db.ExecuteScalar<dynamic>(sql, objparameters);

            return null;
        }



        public object GetDeleteAction(object dbOperator, params object[] objparameters)
        {
            if (objparameters == null)
            {
                return 0;
            }
            var db = dbOperator as MemoryOrm;
            var reint = 0;
            foreach (var o in objparameters)
            {
                var table = db.DB.FirstOrDefault(p => p.TableName == o.GetType().ReflectedType.Name);
                if (table != null)
                {
                    var en = table.DataList.GetEnumerator();

                    while (en.MoveNext())
                    {
                        var objcurrent = en.Current;
                        if (objcurrent.AsDynamic().Id == o.AsDynamic().Id)
                        {
                            table.DataList.Remove(objcurrent);
                            reint++;
                        }
                    }


                }
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

            var db = new MemoryOrm(connectionstring);
            //db.Insertable(new SqlBuilderAccessory()).ExecuteCommandAsync
            return db;
        }
        public object[] GetCeateActionParamters()
        {
            var connectionstring = this.ConnectionConfig.Connectionstring;
            var provider = "memory";
            return new object[] { connectionstring, provider };
        }

        public object GetInsertAction(object dbOperator, params object[] objparameters)
        {
            var db = dbOperator as MemoryOrm;
            var reint = 0;

            foreach (var obj in objparameters)
            {
                var table = db.DB.FirstOrDefault(p => p.TableName == obj.GetType().ReflectedType.Name);
                if (table != null)
                {
                    table.DataList.Add(obj);
                    reint++;
                }

            }
            return reint;
        }

        //public object GetOpenSessionAction(object dbOperator, params object[] objparameters)
        //{
        //    return dbOperator;
        //}
        public List<dynamic> GetQuertyAction(object dbOperator, string sql, params object[] args)
        {

            var db = dbOperator as MemoryOrm;
            //var query = db.Query<dynamic>(sql, args).ToList();
            var table = db.DB.FirstOrDefault(p => p.TableName == sql);
            if (table != null)
            {
                foreach (var s in table.DataList)
                {

                }
                return table.DataList.Cast<dynamic>().ToList();
            }
            return null;
        }

        public QueryResult GetQueryPageAction(object dbOperator, string sql, int startpage, int pagesize, params object[] args)
        {
            if (pagesize <= 0)
            {
                pagesize = 100;
            }
            // var query = db.SkipTake<dynamic>(startpage * pagesize, pagesize, sql, args);
            var alldata = GetQuertyAction(dbOperator, sql, args);
            var tagelist = alldata.Skip(startpage * pagesize).Take(pagesize);

            var relist = new QueryResult()
            {
                DataList = tagelist.ToList(),
                currentIndex = startpage + 1,
                PageSize = pagesize,
                TotalPage = alldata.Count
            };
            return relist;
        }


        public int GetUpdatection(object dbOperator, params object[] objparameters)
        {
            var db = dbOperator as MemoryOrm;
            var reint = 0;
            foreach (var o in objparameters)
            {
                var table = db.DB.FirstOrDefault(p => p.TableName == o.GetType().ReflectedType.Name);
                if (table != null)
                {
                    var en = table.DataList.GetEnumerator();

                    while (en.MoveNext())
                    {
                        var objcurrent = en.Current;
                        if (objcurrent.AsDynamic().Id == o.AsDynamic().Id)
                        {
                            objcurrent = o;
                            reint++;
                        }
                    }
                }
            }
            return reint;
        }

        public IEnumerable<T> GetSqlQueryAction<T>(object dbOperator, string sql, params object[] args) where T : class, new()
        {
            var db = dbOperator as MemoryOrm;
            //var query = db.Query<dynamic>(sql, args).ToList();
            var table = db.DB.FirstOrDefault(p => p.TableName == sql);
            if (table != null)
            {
                //foreach (var s in table.DataList)
                //{

                //}
                return table.DataList.Cast<T>().ToList();
            }
            return null;
        }

        public IEnumerable<T> QueryList<T>(object dbOperator, Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var db = dbOperator as MemoryOrm;
            //var query = db.Query<dynamic>(sql, args).ToList();
            var table = db.DB.FirstOrDefault(p => p.TableName == typeof(T).Name);
            if (table != null)
            {
                //foreach (var s in table.DataList)
                //{

                //}
                return table.DataList.Cast<T>().Where(predicate.Compile());
            }
            return null;
        }
    }
}
