using Fasterflect;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UniOrm;
using UniOrm.Common;
using Autofac;
using System.IO;
using System.Collections.Concurrent;
using UniOrm.Model;
using UniOrm.Core;
using UniOrm.Application;
using UniOrm.Model.DataService;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using UniOrm.Loggers;
using RazorLight;

namespace UniOrm
{
    public class APP
    {
        public static ServiceProvider ApplicationServices;
        public static RuntimeCache RuntimeCache;
        public static ContainerBuilder Builder = new ContainerBuilder();
        public static IResover Container;
        public static Dictionary<string, Assembly> Dlls = new Dictionary<string, Assembly>();
        public static List<string> DynamicReferenceDlls = new List<string>();
        public static Dictionary<string, RuntimeCode> RuntimeCodes = new Dictionary<string, RuntimeCode>();
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public static List<ComposeEntity> Composeentitys = new List<ComposeEntity>();
        public static List<ComposeTemplate> ComposeTemplates = new List<ComposeTemplate>();
        public static List<AConFlowStep> AConFlowSteps = new List<AConFlowStep>();
        public static DefaultModuleManager ModuleManager { get; set; } = new DefaultModuleManager();
        private static RazorLightEngine razorengine = null;
        public static RazorLightEngine Razorengine
        {
            get
            {
                if (razorengine == null)
                {
                    razorengine = new RazorLightEngineBuilder().UseMemoryCachingProvider().Build();
                    var isok = razorengine.Options.Namespaces.Add("UniOrm");
                    isok = razorengine.Options.Namespaces.Add("UniOrm.Application");
                    isok = razorengine.Options.Namespaces.Add("UniOrm.Common");
                    isok = razorengine.Options.Namespaces.Add("UniOrm.Model");
                    isok = razorengine.Options.Namespaces.Add("UniOrm.Startup.Web");

                    isok = razorengine.Options.Namespaces.Add("System");
                    isok = razorengine.Options.Namespaces.Add("System.Web");
                    isok = razorengine.Options.Namespaces.Add("System.IO");
                    isok = razorengine.Options.Namespaces.Add("System.Text");
                    isok = razorengine.Options.Namespaces.Add("System.Text.Encodings");
                    isok = razorengine.Options.Namespaces.Add("System.Text.RegularExpressions");
                    isok = razorengine.Options.Namespaces.Add("System.Collections.Generic");
                    isok = razorengine.Options.Namespaces.Add("System.Diagnostics");
                    isok = razorengine.Options.Namespaces.Add("System.Linq");
                    isok = razorengine.Options.Namespaces.Add("System.Security.Claims");
                    isok = razorengine.Options.Namespaces.Add("System.Threading");
                    isok = razorengine.Options.Namespaces.Add("System.Threading.Tasks");
                    isok = razorengine.Options.Namespaces.Add("System.Reflection");
                    isok = razorengine.Options.Namespaces.Add("System.Dynamic");
                    isok = razorengine.Options.Namespaces.Add("System.Diagnostics");
                    isok = razorengine.Options.Namespaces.Add("System.Web.Mvc.ViewPage");
                    isok = razorengine.Options.Namespaces.Add("System.Linq.Expressions");
                    isok = razorengine.Options.Namespaces.Add("System.Xml");
                    isok = razorengine.Options.Namespaces.Add("System.Xml.Linq");
                    isok = razorengine.Options.Namespaces.Add("System.Configuration");

                    isok = razorengine.Options.Namespaces.Add("System.Data");
                    isok = razorengine.Options.Namespaces.Add("System.Data.SqlClient");
                    isok = razorengine.Options.Namespaces.Add("System.Data.Common");
                    isok = razorengine.Options.Namespaces.Add("System.Data.OleDb");
                    isok = razorengine.Options.Namespaces.Add("System.Globalization");
                    isok = razorengine.Options.Namespaces.Add("System.Net");
                    isok = razorengine.Options.Namespaces.Add("System.Net.Http");
                    isok = razorengine.Options.Namespaces.Add("System.Net.Http.Headers");
                    isok = razorengine.Options.Namespaces.Add("System.Net.Mail");
                    isok = razorengine.Options.Namespaces.Add("System.Net.Security");
                    isok = razorengine.Options.Namespaces.Add("System.Net.Sockets");
                    isok = razorengine.Options.Namespaces.Add("System.Net.WebSockets");
                    isok = razorengine.Options.Namespaces.Add("System.Drawing");
                    isok = razorengine.Options.Namespaces.Add("System.Drawing.Printing");
                    isok = razorengine.Options.Namespaces.Add("Newtonsoft.Json");
                    isok = razorengine.Options.Namespaces.Add("Newtonsoft.Json.Linq");
                    isok = razorengine.Options.Namespaces.Add("System.Numerics");
                    isok = razorengine.Options.Namespaces.Add("Microsoft.AspNetCore.Authentication");
                    isok = razorengine.Options.Namespaces.Add("Microsoft.AspNetCore.Authorization");
                    isok = razorengine.Options.Namespaces.Add("Microsoft.Extensions.DependencyInjection");
                    isok = razorengine.Options.Namespaces.Add("Microsoft.AspNetCore.Mvc");

                }
                return razorengine;
            }
        }
        public static Dictionary<string, MethodInfo> MethodInfos = new Dictionary<string, MethodInfo>();
        //public  List<RuntimeModel> RuntimeModels = new List<RuntimeModel>();
        public static ConcurrentDictionary<string, object> StepResults = new ConcurrentDictionary<string, object>();
        readonly static string logName = "ACon.Unver.Manager";
        static APP()
        {
            //var allModules = ModuleManager.RegistedModules;
            //foreach (var m in allModules)
            //{
            //    m.Init();
            //}
        }

        public static DefaultUser LoginDefaultUser(string username, string password)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                var strResult = BitConverter.ToString(result);
                password = strResult.Replace("-", "");

            }
            return Container.Resolve<ICodeService>().GetDefaultUser(username, password);
        }
        private static AppConfig _AppConfig;
        public static AppConfig AppConfig
        {
            get
            {
                if (_AppConfig == null)
                {
                    var configpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config\\system.json");
                    var configroot = JToken.Parse(File.ReadAllText(configpath));
                    _AppConfig = JsonConvert.DeserializeObject<AppConfig>(configroot["App"].ToString());
                }
                return _AppConfig;
            }
        }

        public static AdminUser LoginAdmin(string username, string password)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                var strResult = BitConverter.ToString(result);
                password = strResult.Replace("-", "");

            }
            return Container.Resolve<ICodeService>().GetAdminUser(username, password);
        }
        public static string GetDicstring(string key)
        {
            var item = AppConfig.SystemDictionaries.FirstOrDefault(p => p.KeyName == key);
            if (item != null)
            {
                return item.Value;
            }
            return string.Empty;
        }
        public static IGodWorker GetWorker()
        {
            return Container.Resolve<IGodWorker>();
        }



        public static void RegisterAutofacModule(Autofac.Module moudle)
        {

            Builder.RegisterModule(moudle);
        }

        public static void RegisterAutofacAssemblies(IEnumerable<Assembly> modulesAssembly)
        {
            if (modulesAssembly != null)
            {

                foreach (var m in modulesAssembly)
                {
                    Builder.RegisterAssemblyModules(m);
                }
            }
        }
        public static void RegisterAutofacModuleTypes()
        {
            var allModules = ModuleManager.RegistedModules;
            foreach (var m in allModules)
            {
                var mms = m.GetAutofacModules();
                foreach (var item in mms)
                {
                    Builder.RegisterModule(item);
                }
            }
            //loger
        }
        public static void ClearCache()
        {

            RuntimeCache.Clear();
        }
        public static RuntimeCode FindOrAddRumtimeCode(string guid)
        {
            if (RuntimeCodes.ContainsKey(guid))
            {
                return RuntimeCodes[guid];
            }
            return null;
        }

        public static MethodInfo GetMethodFromConfig(bool IsPlugin, string libname, string typename, string methodName)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            if (!IsPlugin)
            {
                dir = Path.Combine(dir, "Plugins");
            }
            var filepath = Path.Combine(dir, libname);
            Assembly assembley;
            if (!Dlls.ContainsKey(filepath))
            {
                assembley = Assembly.LoadFrom(filepath);
                Dlls.Add(filepath, assembley);
            }
            else
            {
                assembley = Dlls[filepath];
            }
            Type type;
            var alltypename = typename;
            if (!Types.ContainsKey(alltypename))
            {
                type = assembley.GetType(alltypename);
                Types.Add(alltypename, type);
            }
            else
            {
                type = Types[alltypename];
            }
            MethodInfo method;
            var allMethodName = string.Concat(alltypename, ".", methodName);
            if (!MethodInfos.ContainsKey(allMethodName))
            {
                //if( cons.IsStatic)
                //{
                method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
                // }

                MethodInfos.Add(allMethodName, method);
            }
            else
            {
                method = MethodInfos[allMethodName];
            }

            return method;
        }
        public static void ConfigureSiteAllModulesServices(IServiceCollection services)
        {
            var allModules = ModuleManager.RegistedModules;
            foreach (var m in allModules)
            {
                m.ConfigureSiteServices(services);
            }
        }

        public static void Startup(IConfiguration configuration)
        {
            var allModules = ModuleManager.RegistedModules;
            foreach (var m in allModules)
            {
                m.Startup(configuration);
            }
        }


        public static void ConfigureSiteAllModules(IApplicationBuilder app)
        {
            var allModules = ModuleManager.RegistedModules;
            foreach (var m in allModules)
            {
                var mms = m.GetAutofacModules();
                foreach (var item in mms)
                {
                    Builder.RegisterModule(item);
                }
            }
        }


        public static object OpenDBSession(object dbFactory, string ormname, string connectionstring)
        {

            var adp = dbFactory.GetIndexer(ormname);
            //  .GetIndexer(ormname);

            if (!string.IsNullOrEmpty(connectionstring))
            {
                return adp.CallMethod("CreateDefaultInstance", connectionstring);
            }
            else
            {
                return adp.CallMethod("CreateDefaultInstance");
            }


        }

        public static object GetData(string ssql, params object[] inParamters)
        {

            object DynaObject;
            var dbFactory = Container.Resolve<IDbFactory>();
            var ormname = AppConfig.UsingDBConfig.OrmName;
            var cacheormnameKey = string.Concat(AppConfig.UsingDBConfig.Connectionstring);
            object dbgrouder = APP.RuntimeCache.GetOrCreate(cacheormnameKey, entry =>
            {
                object dbgroudernew = OpenDBSession(dbFactory, ormname, null);
                APP.RuntimeCache.Set(cacheormnameKey, dbgroudernew);
                return dbgroudernew;
            });

            if (dbgrouder == null)
            {
                Logger.LogError(logName, "dbgrouder is null");
            }
            //var usingdbtype=  config.GetValue<int>("App", "UsingDBConfig", "DBType")
            //  var dbtype = (DBType)_dbtype;
            //  if (dbtype == config.)

            var objParams2 = inParamters;
            //if (!string.IsNullOrEmpty(s.ArgNames))
            //{
            //    var args = s.ArgNames.Split(',');
            //    foreach (var aarg in args)
            //    {
            //        object obj = aarg;
            //        if (aarg.StartsWith("&"))
            //        {
            //            obj = newrunmodel.Resuce(aarg);

            //        }
            //        objParams2.Add(obj);
            //    }
            //}
            if (objParams2 == null || objParams2.Length == 0)
            {
                DynaObject = APP.GetData(dbgrouder, ssql);
            }
            else
            {
                DynaObject = APP.GetData(dbgrouder, ssql, objParams2);
            }

            return DynaObject;

        }

        public static object GetData(object db, string ssql, params object[] inParamters)
        {

            List<object> allparas = new List<object>();
            allparas.Add(ssql);
            allparas.AddRange(inParamters);

            var alldatas = db.AsDynamic().Query(ssql, inParamters);

            return alldatas;

        }
        //public static object GetData<T>(  string ssql, params object[] inParamters)
        //{
        //    Type t = Type.GetType("AConState.DataService.ICodeService");
        //    Container.Resolve(t).CallMethod("GetSimpleCode", inParamters);
        //    //var product = CSScript.CreateFunc<int>(@"int Product(int a, int b)
        //    //                             {
        //    //                                 return a * b;
        //    //                             }");
        //    return null;
        //}
        public object Invoke<T>(params string[] commands)
        {
            var t = typeof(T);

            //t.
            return null;
        }

        public static void SetServiceProvider()
        {
            var allModules = ModuleManager.RegistedModules;
            foreach (var m in allModules)
            {
                m.SetServiceProvider(ApplicationServices);
            }
        }
    }
}
