using FluentMigrator;
using System;

namespace DataMigrationiHistrory
{
    [Migration(10000000000001)]
    public class Init : Migration
    {
        public override void Up()
        {

            Create.Table("pigcms_adma")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("info").AsString(200)
                .WithColumn("title").AsString(300) 
                ;
            
        }

        public override void Down()
        {
            Delete.Table("SystemACon");
           
        }
    }
}
