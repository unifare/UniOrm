using SqlKata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UniOrm
{
    public class DataGrouderT<T>  where T : class, new()
    {
        public List<T> Query(Query query)
        {
            var sss = Parent.ToSql(query);

            return Query(sss.Sql, sss.Bindings.ToArray());

        }
        public List<T> Query(string sql, params object[] args)
        {
            return Parent.OrmAdaptor.GetSqlQueryAction<T>(Parent.OrmObject, sql, args).ToList();
        }
        public DataGrounder Parent { get; set; }
    }
}
