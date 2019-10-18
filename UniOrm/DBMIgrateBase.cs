using FluentMigrator;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UniOrm
{
    public class DBMIgrateBase: Migration
    {
        private static string _Prefixname;
        public static string Prefixname
        {
            get
            {
                if (_Prefixname == null)
                {
                    var configpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config\\system.json");
                    var configroot = JToken.Parse(File.ReadAllText(configpath));
                    _Prefixname = configroot["App"]["UsingDBConfig"]["DefaultDbPrefixName"].ToString();

                }
                return _Prefixname;
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            throw new NotImplementedException();
        }

        protected string WholeTableName(string tablename)
        {
            return Prefixname + tablename;
        }
    }
}
