using FluentMigrator;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniOrm.Common;

namespace UniOrm.DataMigrationiHistrory
{
    [Migration(2)]
    public class DBform : DBMIgrateBase
    {

        public override void Up()
        {

            //Create.Table(WholeTableName("SystemACon"))
            //    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            //    .WithColumn("AppName").AsString(200)
            //    .WithColumn("AppDiscription").AsString(300)
            //    .WithColumn("AppConfigs").AsString().Nullable()
            //    .WithColumn("VersionNum").AsString(100).Nullable()
            //    .WithColumn("UpdateUrl").AsString(600).Nullable()
            //    .WithColumn("UpdateParamter").AsString(600).Nullable()
            //    .WithColumn("Author").AsString(100).Nullable()
            //    .WithColumn("LastRunTime").AsDateTime().Nullable()
            //     .WithColumn("CreateTime").AsDateTime().Nullable()
                ;
          
        }

        public override void Down()
        {
           // Delete.Table(WholeTableName("SystemACon"));
            
        }
    }
}
