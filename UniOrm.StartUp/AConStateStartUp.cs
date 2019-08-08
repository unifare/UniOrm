using Autofac;
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
    public static class AConStateStartUp
    {
        private static IServiceProvider autofacServiceProvider;


        static bool s_isInit = false;
        private static readonly string loggerName = "AConStateStartUp";

        //internal static string sysconstring = "Data Source = ./config/aconstate.db";
        //internal static string codeormtype = "sqlsugar";
        /// <summary>
        /// 扫描后端
        /// </summary>
        /// <param name="filePath">bin目录</param>
        private static string[] ScanBack(string dlldir)
        {

            return Directory.GetFiles(dlldir, "*.dll");
        }
        public static AppConfig AppConfig { get; set; }
        public static DcConnectionConfig SystemConConfig { get; set; }
        public static void EnsureDaContext( )
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
        public static void EnsureDaContext(params Assembly[] assemblies)
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
        public static IServiceProvider InitAutofac(this IServiceCollection services, List<Autofac.Module> modules)
        {
            if (s_isInit)
            {
                return autofacServiceProvider;
            }
            var builder = Manager.Builder;

            builder.RegisterModule(new AutofacModule());
            if (modules != null)
            {
                foreach (var m in modules)
                {
                    Manager.Builder.RegisterModule(m);
                }
            }
            builder.RegisterInstance<IDbFactory>(new DbFactory());
            builder.Populate(services);

            var container = builder.Build();
            var memoryCache = container.Resolve<IMemoryCache>();
            Manager.RuntimeCache = new RuntimeCache(memoryCache);
            var currentAssembly = typeof(PatePocoOrmAdaptor).GetTypeInfo().Assembly;
            var basedir = AppDomain.CurrentDomain.BaseDirectory;
            //foreach (var ea in Directory.GetFiles(  basedir).Where(p=>p.EndsWith(".dll")))
            //{
            //    Manager.Dlls.Add(ea)
            //}
            var ormProviders = currentAssembly.GetImplementationsOf<IOrmAdaptor>();


            var ormfactory = container.Resolve<IDbFactory>();

            IConfig config = container.Resolve<IConfig>();
            AppConfig = config.GetValue<AppConfig>("App");
            SystemConConfig = AppConfig.UsingDBConfig;//.Connectionstrings.FirstOrDefault(p => p.Name == "sys_default");

            var listtypedModels = ReadTypeFromConfig(AppConfig.EFRegestedModels);
            foreach (var orm in ormProviders)
            {
                orm.RegistedModelTypes.AddTypes(listtypedModels);
                orm.ConnectionConfig = SystemConConfig;
                ormfactory.AddOrm(orm);
            } 
            autofacServiceProvider = container.Resolve<IServiceProvider>();
            var systemResover = new AutofacResover() { Container = container };
            builder.RegisterInstance<IResover>(systemResover);

            Manager.Container = systemResover;
            return autofacServiceProvider;
        }

        private static List<Type> ReadTypeFromConfig(List<RegestedModel> regestedModels)
        {
            var listtypedModels = new List<Type>();
            foreach (var m in regestedModels)
            {
                var assembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, m.Dll));
                foreach (var ty in m.TypeNames)
                {
                    var t = assembly.GetType(ty);
                    if (t == null)
                    {
                        Logger.LogWarn(loggerName, "InitAutofac -> Load Type {0} not in {1}", ty, m);
                        continue;
                    }
                    listtypedModels.Add(t);
                }

            }
            return listtypedModels;
        }
    }
}
