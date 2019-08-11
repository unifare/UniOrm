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

namespace UniOrm
{
    public class SuperManager
    {
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
        public static Dictionary<string, MethodInfo> MethodInfos = new Dictionary<string, MethodInfo>();
        //public  List<RuntimeModel> RuntimeModels = new List<RuntimeModel>();
        public static ConcurrentDictionary<string, object> StepResults = new ConcurrentDictionary<string, object>();
        readonly static string logName = "ACon.Unver.Manager";
        public SuperManager()
        {
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
            var item = ApplicationStartUp.AppConfig.SystemDictionaries.FirstOrDefault(p => p.KeyName == key);
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
        public static IModule GetModule(ModuleName name, string othername = null)
        {
            return DefaultModuleManager.RegistedModules.FirstOrDefault(p => p.ModuleName == name.ToString());
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
    }
}
