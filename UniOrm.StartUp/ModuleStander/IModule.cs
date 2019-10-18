using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UniOrm.Common;

namespace UniOrm.Application
{
    public interface IModule
    {
      
        string DllPath { get; set; }
        string ModuleName { get; set; }
        bool Init();
        AppConfig ModuleAppConfig { get; set; }
        void EnsureDaContext();
        List<Type> ModelType();
        void Startup(IConfiguration configuration);

        List<string> ModelTypeStrings();

        DcConnectionConfig dcConnectionConfig { get; set; }
        List<Type> FunctionalTypes { get; set; }
        string GetDBTablePrefixConfig();
        bool MigrateDB();

        void SetServiceProvider(ServiceProvider serviceProvider);
        List<IGodWorker> GodWorkers { get; set; }
        List<Module> GetAutofacModules();
        RequireItemCollection RequireItems { get; set; }

        DbMigrationUnit ModuleMigrationUnit { get; set; }

        void ConfigureSiteServices(IServiceCollection services);

        void ConfigureSite(IApplicationBuilder app);
    }
}
