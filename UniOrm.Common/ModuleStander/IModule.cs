using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UniOrm.Common;

namespace UniOrm.Common
{
    public interface IModule
    {
        ContainerBuilder Builder { get; }
        IResover Container { get; }
        string DllPath { get; set; }
        string ModuleName { get;  }
        bool Init();
        AppConfig ModuleAppConfig { get; set; }
        void EnsureDaContext();
        List<Type> ModelType();
        void Startup(IConfiguration configuration);
        DB DB { get; set; }
        List<string> ModelTypeStrings();

        DbConnectionConfig dcConnectionConfig { get; set; }
        List<Type> FunctionalTypes { get; set; }
        string GetDBTablePrefixConfig();
        bool MigrateDB();

        void SetServiceProvider(ServiceProvider serviceProvider);

        List<Module> GetAutofacModules();

        void RegisterAutofacTypes();
        //RequireItemCollection RequireItems { get; set; }

        //DbMigrationUnit ModuleMigrationUnit { get; set; }

        void ConfigureSiteServices(IServiceCollection services);


    }
}
