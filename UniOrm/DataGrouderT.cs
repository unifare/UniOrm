using SQLBuiler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UniOrm
{
    public class DataGrouderT<T> where T : class, new()
    {
        public Query From(string tableName)
        {
            return new Query(Parent.OrmAdaptor.ConnectionConfig.DefaultDbPrefixName + tableName);
        }
        public List<T> QueryFrom(string tableName)
        {
            var query = new Query(Parent.OrmAdaptor.ConnectionConfig.DefaultDbPrefixName + tableName);

            return QueryFrom(query);
        }
        public List<T> QueryFrom()
        {
            var query = new Query(Parent.OrmAdaptor.ConnectionConfig.DefaultDbPrefixName + typeof(T).Name);
            return QueryFrom(query);
        }

        protected List<T> QueryFrom(Query q)
        {
          
            return Query(q);
        }

        internal List<T> Query(Query query)
        {
            var sss = Parent.ToSql(query);

            return Query(sss.Sql, sss.Bindings.ToArray());

        }

        public List<T> Query(string sql, params object[] args)
        {
            return Parent.OrmAdaptor.GetSqlQueryAction<T>(Parent.OrmObject, sql, args).ToList();
        }

       
        public QueryResult QueryPage<T>(string sql, int pindex, int psize, params object[] args) where T : class, new()
        {
            return Parent.OrmAdaptor.GetSqlQueryPageAction<T>(Parent.OrmObject, sql, pindex, psize, args);

        }

        public DataGrounder Parent { get; set; }
    }
}
