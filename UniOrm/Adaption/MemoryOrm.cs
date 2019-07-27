using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace UniOrm.Adaption
{
    public class Table
    {
        public string TableName { get; set; }
        public IList DataList { get; set; }
    }

      

    public class MemoryOrm
    {
        public string ConnectionString;
        public MemoryOrm(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<Table>  DB { get;set;}
        //public int Execute(string sql, params object[] paramters)
        //{
        //    //DB.Where
        //}
        //public void AddTable(IList list)
        //{
        //    var ssd= 
        //   // DB.Add(list);
        //    var ss = new List<int>();
        //    DB.Add(ss);
        //    var seees = new List<string>();

        //    DB.Add(seees);
        //}
    }
}
