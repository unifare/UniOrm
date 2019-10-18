
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using FlunentMigratorFactory;
using System.IO;
using System.Reflection;
using UniOrm.Adaption;
using Microsoft.Extensions.Caching.Memory;
using UniOrm.Core;
using UniOrm;
using UniOrm.Common;
using UniOrm.Loggers;
using UniOrm.DataMigrationiHistrory;


namespace UniOrm.Application
{
    public class DbMigrationUnit
    {
        public static void EnsureDaContext(DcConnectionConfig SystemConConfig)
        {
            var dbtype = (DBType)SystemConConfig.DBType;
            if (dbtype != DBType.InMemory)
            {
                var fuType = FlunentDBType.Sqlite;

                switch (dbtype)
                {
                    case DBType.Sqlite:
                        fuType = FlunentDBType.Sqlite;
                        break;
                    case DBType.SqlServer:
                        fuType = FlunentDBType.MsSql;
                        break;
                    case DBType.Mysql:
                        fuType = FlunentDBType.MySql4;
                        break;
                    case DBType.Postgre:
                        fuType = FlunentDBType.Postgre;
                        break;
                }
                MigratorFactory.CreateServices(fuType, SystemConConfig.Connectionstring, MigrationOperation.MigrateUp, 0, typeof(Init).Assembly);
            }
        }
        public static void EnsureDaContext(DcConnectionConfig SystemConConfig, params Assembly[] assemblies)
        {
            var dbtype = (DBType)SystemConConfig.DBType;
            if (dbtype != DBType.InMemory)
            {
                var fuType = FlunentDBType.Sqlite;

                switch (dbtype)
                {
                    case DBType.Sqlite:
                        fuType = FlunentDBType.Sqlite;
                        break;
                    case DBType.SqlServer:
                        fuType = FlunentDBType.MsSql;
                        break;
                    case DBType.Mysql:
                        fuType = FlunentDBType.MySql4;
                        break;
                    case DBType.Postgre:
                        fuType = FlunentDBType.Postgre;
                        break;
                }
                MigratorFactory.CreateServices(fuType, SystemConConfig.Connectionstring, MigrationOperation.MigrateUp, 0, assemblies);
            }
        }

    }
}
