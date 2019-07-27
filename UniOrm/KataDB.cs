using SqlKata;
using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm
{
    public class KataDB
    {
        public static Query From(string tableName)
        {
            return new Query(tableName);
        }
        public static Query From<T>( )
        {
            return new Query(typeof(T).Name);
        }
    }
}
