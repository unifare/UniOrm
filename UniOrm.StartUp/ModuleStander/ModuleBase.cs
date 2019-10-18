using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using UniOrm.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniOrm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace UniOrm.Application.ModuleStander
{
    public abstract class ModuleBase : IModule
    {
        public static ServiceProvider ServiceProvider { get; set; }
        public static IConfiguration Configuration { get; set; }
        public virtual void Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public abstract void EnsureDaContext(); 

        public abstract string DllPath { get; set; }
        public string ModuleName { get; set; }
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public RequireItemCollection RequireItems { get; set; } 
        public List<IGodWorker> GodWorkers { get; set; }
        /// <summary>
        /// 扫描后端
        /// </summary>
        /// <param name="filePath">bin目录</param>
        private static string[] ScanBack(string dlldir)
        {
            return Directory.GetFiles(dlldir, "*.dll");
        }

        public abstract void SetServiceProvider(ServiceProvider serviceProvider);
        public ModuleBase()
        {
            Types = new Dictionary<string, Type>();
            //Init();
        }
        public AppConfig GetAppConfig()
        {
            return GodWorker.appConfig;
        }

        public abstract List<Autofac. Module> GetAutofacModules();

        public virtual string GetDBTablePrefixConfig()
        {
            return GodWorker.appConfig.UsingDBConfig.DefaultDbPrefixName;
        }

        static string configPath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString() + ".config";
        static Configuration MyConfiguration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap()
        {
            ExeConfigFilename = configPath
        }, ConfigurationUserLevel.None);

        public string GetModuleConfig(string key)
        {
            return MyConfiguration.AppSettings.Settings[key] == null ? "" : MyConfiguration.AppSettings.Settings[key].Value;
        }
        public virtual bool Init()
        {
            SetModuleAppConfig();
            var configFileDir =  GetModuleConfig("IsUsingAppDB").ToBool();
            dcConnectionConfig = GetAppConfig().UsingDBConfig;
            return true;
        }

        private void SetModuleAppConfig()
        {
            var configFileDir = GetModuleConfig("configFileDir");
            var configFilePath = GetModuleConfig("configFile");

            if (!string.IsNullOrEmpty(configFileDir))
            {
                var phyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileDir);
                if (!Directory.Exists(phyDir))
                {
                    Directory.CreateDirectory(phyDir);
                }

            }
            configFilePath = configFileDir + "/" + configFilePath;
            var configpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFilePath);
            var configroot = JToken.Parse(File.ReadAllText(configpath));
            ModuleAppConfig = JsonConvert.DeserializeObject<AppConfig>(configroot["App"].ToString());
        }

        public abstract AppConfig ModuleAppConfig { get; set; }

        public abstract List<Type> ModelType();

        public abstract List<string> ModelTypeStrings();

        public abstract DcConnectionConfig dcConnectionConfig { get; set; }

        public abstract List<Type> FunctionalTypes { get; set; }
        public virtual bool MigrateDB()
        {
            return true;
        }

        public abstract void ConfigureSiteServices(IServiceCollection services);

        public abstract void ConfigureSite(IApplicationBuilder app);

        public DbMigrationUnit ModuleMigrationUnit { get; set; }
    }
}
