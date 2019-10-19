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
    public static class ApplicationStartUp
    {
        private static IServiceProvider autofacServiceProvider;


        static bool s_isInit = false;
        private static readonly string loggerName = "AConStateStartUp";

        static ApplicationStartUp()
        {

        }

        //internal static string sysconstring = "Data Source = ./config/aconstate.db";
        //internal static string codeormtype = "sqlsugar";
        /// <summary>
        /// 扫描后端
        /// </summary>
        /// <param name="filePath">bin目录</param>
        private static string[] GetAllPluginDlls(string dlldir)
        {

            return Directory.GetFiles(dlldir, "*Plugin.dll");
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
       
      
        public static IServiceProvider InitAutofac(this IServiceCollection services, IEnumerable<Assembly> modulesAssembly)
        {
            if (s_isInit)
            {
                return autofacServiceProvider;
            }
            services.AddAutofac();
            var builder = APP.Builder;
            APP.RegisterAutofacModule(new AutofacModule()); 
            APP.RegisterAutofacModuleTypes();
            APP.RegisterAutofacAssemblies(modulesAssembly);
          
            builder.RegisterInstance<IDbFactory>(new DbFactory());

            builder.Populate(services);

            var container = builder.Build();

            ///////////using /////////////////////

            IConfig config = container.Resolve<IConfig>();
         
            var memoryCache = container.Resolve<IMemoryCache>();
            APP.RuntimeCache = new RuntimeCache(memoryCache);
            var currentAssembly = typeof(PatePocoOrmAdaptor).GetTypeInfo().Assembly;
            var basedir = AppDomain.CurrentDomain.BaseDirectory;
            //foreach (var ea in Directory.GetFiles(  basedir).Where(p=>p.EndsWith(".dll")))
            //{
            //    Manager.Dlls.Add(ea)
            //}
            var ormProviders = currentAssembly.GetImplementationsOf<IOrmAdaptor>();


            var ormfactory = container.Resolve<IDbFactory>();


            var systemConConfig = APPCommon.AppConfig.UsingDBConfig;//.Connectionstrings.FirstOrDefault(p => p.Name == "sys_default");

            var listtypedModels = ReadTypeFromConfig(APPCommon.AppConfig.EFRegestedModels);
            foreach (var orm in ormProviders)
            {
                orm.RegistedModelTypes.AddTypes(listtypedModels);
                orm.ConnectionConfig = systemConConfig;
                ormfactory.AddOrm(orm);
            }
            autofacServiceProvider = container.Resolve<IServiceProvider>();
            var systemResover = new AutofacResover() { Container = container };
            builder.RegisterInstance<IResover>(systemResover);

            APP.Container = systemResover;
            return autofacServiceProvider;
        }
    }
}
