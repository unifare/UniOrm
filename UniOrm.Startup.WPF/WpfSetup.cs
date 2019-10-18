using Microsoft.Extensions.DependencyInjection;
using System; 
using System.IO; 
using UniOrm.Common; 
using UniOrm;
using UniOrm.Application; 
using Autofac; 
using UniOrm.Core;
using System.Linq; 
using UniOrm.Loggers;

namespace UniOrm.Startup.WPF
{
    public static class WebSetup
    {
        private static AppConfig appConfig = null;
        private const string LoggerName = "WebSetup";
        // This method gets called by the runtime. Use this method to add services to the container.
        public static IServiceProvider ConfigureServices(this IServiceCollection services)
        {
            //register configer
            JsonConfig jsonConfig = new JsonConfig();
            var pa = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config\\system.json");
            jsonConfig.Source = File.ReadAllText(pa);
            var builder = new ContainerBuilder();
            builder.RegisterInstance<IConfig>(jsonConfig);
            var tempcontainer = builder.Build();

        
            // 配置授权
            var config = tempcontainer.Resolve<IConfig>();
            appConfig = config.GetValue<AppConfig>("App");
            var signingkey = GetDicstring(appConfig, "JWT.IssuerSigningKey");
            var backendfoldername = GetDicstring(appConfig, "backend.foldername");
            var identityserver4url = GetDicstring(appConfig, "Identityserver4.url");
            var Identityserver4ApiResouceKey = GetDicstring(appConfig, "Identityserver4.ApiResouceKey");
            var idsr4_ClientId = GetDicstring(appConfig, "idsr4_ClientId");
            var idsr4_ClientSecret = GetDicstring(appConfig, "idsr4_ClientSecret");
            var idsr4_ReponseType = GetDicstring(appConfig, "idsr4_ReponseType");
            var OauthClientConfig_scopes = GetDicstring(appConfig, "OauthClientConfig_scopes");
            var IsUsingIdentityserverClient = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingIdentityserverClient"));
            var IsUsingIdentityserver4 = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingIdentityserver4"));
            var isAllowCros = Convert.ToBoolean(GetDicstring(appConfig, "isAllowCros"));
            var AllowCrosUrl = GetDicstring(appConfig, "AllowCrosUrl");
            var isEnableSwagger = Convert.ToBoolean(GetDicstring(appConfig, "isEnableSwagger")); ;
            //services.AddMvcCore().AddAuthorization().AddJsonFormatters(); 
            var IsUsingLocalIndentity = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingLocalIndentity"));
            // var IsUsingDB = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingDB"));

       

       
             var asses = AppDomain.CurrentDomain.GetAssemblies();
            var we = services.InitAutofac(asses);
            APP.Container.Resolve<IConfig>().GetValue<AppConfig>().ResultDictionary = appConfig.ResultDictionary;
            InitDbMigrate();
             
            return we;
        }
        public static string GetDicstring(AppConfig appConfig, string key)
        {
            var item = appConfig.SystemDictionaries.FirstOrDefault(p => p.KeyName == key);
            if (item != null)
            {
                return item.Value;
            }
            return string.Empty;
        }
        public static void InitDbMigrate()
        {
            DbMigrationUnit.EnsureDaContext(appConfig.UsingDBConfig);
        }
    
    }
}
