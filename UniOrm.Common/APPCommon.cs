﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UniOrm;
using UniOrm.Common;
using Autofac;
using System.IO;
using System.Collections.Concurrent; 
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace UniOrm
{
    public partial class APPCommon
    {
        public static HttpClient Client { get; set; } = new HttpClient();
        public static IConfiguration Configuration { get; set; }
        public static ServiceProvider ApplicationServices; 
        public static ContainerBuilder Builder = new ContainerBuilder();
        public static IResover Container;
        public static Dictionary<string, Assembly> Dlls = new Dictionary<string, Assembly>();
        public static List<string> DynamicReferenceDlls = new List<string>(); 
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>(); 
        public static Dictionary<string, MethodInfo> MethodInfos = new Dictionary<string, MethodInfo>();
        //public  List<RuntimeModel> RuntimeModels = new List<RuntimeModel>();
        public static ConcurrentDictionary<string, object> StepResults = new ConcurrentDictionary<string, object>();
        readonly static string logName = "UniOrm.APPCommon";
        public static RuntimeCache RuntimeCache;
        public static DefaultModuleManager ModuleManager { get; set; } = new DefaultModuleManager();

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
        static APPCommon()
        {
            //var allModules = ModuleManager.RegistedModules;
            //foreach (var m in allModules)
            //{
            //    m.Init();
            //}
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
     
         
        public static object GetData(object db, string ssql, params object[] inParamters)
        {

            List<object> allparas = new List<object>();
            allparas.Add(ssql);
            allparas.AddRange(inParamters);

            var alldatas = db.AsDynamic().Query(ssql, inParamters);

            return alldatas;

        }
   
    
    }
}
