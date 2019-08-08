using Fasterflect;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq; 
using UniOrm;
using UniOrm.Common;
using Autofac;
using UniOrm.ReflectionMagic;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;  
using UniOrm.Model;

namespace UniOrm.Core
{
    public class Manager
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
        //public static List<RuntimeModel> RuntimeModels = new List<RuntimeModel>();
        public static ConcurrentDictionary<string, object> StepResults = new ConcurrentDictionary<string, object>();
        static readonly string logName = "ACon.Unver.Manager";
        public Manager()
        {
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
            if( !IsPlugin)
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


        public static object GetData(object db,  string ssql, params object[] inParamters)
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
        public static object Invoke<T>(params string[] commands)
        {
            var t = typeof(T);

            //t.
            return null;
        }
    }
}
