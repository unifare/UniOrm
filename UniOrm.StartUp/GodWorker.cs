using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using UniOrm.Model.DataService;
using UniOrm.Core;
using UniOrm.Common;
using UniOrm;
using UniOrm.Model;
using UniOrm.Loggers;
using RazorLight;

namespace UniOrm.Application
{
    public class RutimeGlobal
    {
        public RutimeGlobal()
        {
            __P = new List<object>();
        }
        public System.Collections.Generic.List<object> __P;
    }


    public class GodWorker : IGodWorker
    {
   
        public string WorkerName { get; set; }
        readonly static object lockobj = new object();
        static string logName = "AConState.Application.GodMaker";
        //public static Dictionary<string, RuntimeModel> RuntimeModels = new Dictionary<string, RuntimeModel>();
        public static AppConfig appConfig = null;

        // public DefaultModuleManager ModuleManager { get; set; }
        ICodeService CodeService;
        IConfig Config;
        IDbFactory DbFactory;
        public GodWorker(ICodeService codeService, IDbFactory dbFactory, IConfig config)
        {
            WorkerName = Guid.NewGuid().ToString("N");
            DbFactory = dbFactory;
            CodeService = codeService;
            Config = config;
            //ModuleManager = new DefaultModuleManager();
        }


        public async Task Run(params object[] parameters)
        {

            var st = new System.Diagnostics.StackTrace();
            var lastMethod = st.GetFrame(1).GetMethod();
            //var appconfigstring = Config.GetValue<AppConfig>("App").AppConfigs;

            if (appConfig == null)
            {
                //appConfig = SuperManager.Container.Resolve<IApp>
                //{
                //    AppType = "aspnetcore",
                //    Connectionstrings = new List<DcConnectionConfig>()
                //    {
                //        new DcConnectionConfig ()
                //        {
                //            OrmName="PatePoco",
                //            Name="sys_default" ,
                //            Connectionstring = "Data Source = ./config/aconstate.db"
                //        }, new DcConnectionConfig ()
                //        {
                //            OrmName="InMemory",
                //            Name="InMemory_default" ,
                //            Connectionstring = "AconState"
                //        }
                //        , new DcConnectionConfig ()
                //        {
                //            OrmName="EFCore",
                //            Name="EFCore_default" ,
                //            Connectionstring = "Data Source = ./config/aconstate.db"
                //        }
                //    },
                //    OrmTypes = new List<string> { "PatePoco" },
                //    TrigerType = "urlreg"
                //};

            }
            if (appConfig.AppType == "aspnetcore")
            {

                ComposeEntity cons = null;

                cons = FindComposity(appConfig, null);
                if (cons == null)
                {
                    Logger.LogError(logName, "Run -> Find composity returns null ");
                    return;
                }

                var newrunmodel = new RuntimeModel(Config)
                {
                    ComposeEntity = cons,
                    HashCode = cons.GetHash()
                };
                RuntimeModel.StaticResouceInfos["__actioncontext"] = parameters[0];
                RuntimeModel.StaticResouceInfos["__httpcontext"] = parameters[0].GetProp("HttpContext") ;

                RuntimeModel.StaticResouceInfos["__config"] = appConfig;
              
                if (!string.IsNullOrEmpty(cons.Templateid))
                {
                    newrunmodel.ComposeTemplate = FindComposeTemplet(cons.Templateid);
                }

                await RunComposity(parameters[0].GetHashCode(), newrunmodel, DbFactory, CodeService, Config);


            }
            //var context = parameters[0] as ActionExecutingContext;
            //var resp = context.HttpContext.Response;
            //resp.ContentType = "text/html";

            //using (StreamWriter sw = new StreamWriter(resp.Body))
            //{
            //    sw.Write("Write a string to response in WriteResponseWithoutReturn!");
            //} 
        }

        //public void GetRequestHash(ActionExecutingContext actionExecutingContext)
        //{
        //    //actionExecutingContext.HttpContext.req
        //}


        private async static Task RunComposity(int requsetHash, RuntimeModel newrunmodel, IDbFactory dbFactory, ICodeService codeService, IConfig config)
        {
            var cons = newrunmodel.ComposeEntity;
            if (cons.RunMode == RunMode.Coding)
            {
                if (newrunmodel.ComposeTemplate != null)
                {
                    //TODO :add template
                }
                //Manager.RuntimeModels.Add(newrunmodel);
                else
                {
                    var steps = FindSteps(cons.Guid, codeService);

                    foreach (var s in steps)
                    {
                        object rebject = null;
                        object DynaObject = null;

                        var cacheKey = string.Concat(cons.Guid, "_", s.ExcuteType, "_", s.FlowStepType, "_", s.Guid, "_", s.ArgNames);
                        object stepResult = APP.RuntimeCache.GetOrCreate(cacheKey, entry =>
                      {
                          object newobj = null;
                          APP.RuntimeCache.Set(cacheKey, newobj);
                          return newobj;
                      });
                        if (stepResult == null || !s.IsUsingCache)
                        {
                            switch (s.ExcuteType)
                            {
                                case ExcuteType.Syn:
                                    switch (s.FlowStepType)
                                    {
                                        case FlowStepType.Declare:
                                            {
                                                lock (lockobj)
                                                {
                                                    //root.Usings[2].Name.ToString()
                                                    // var rebject2 = Manager.GetData(spec.InParamter1, spec.InParamter2);
                                                    var runcode = APP.FindOrAddRumtimeCode(s.Guid);
                                                    var so_default = ScriptOptions.Default;
                                                    if (runcode == null)
                                                    {
                                                        runcode = new RuntimeCode()
                                                        {
                                                            StepGuid = s.Guid,
                                                            CodeLines = s.ProxyCode,

                                                        };
                                                        List<string> dlls = new List<string>();

                                                        var isref = false;
                                                        string dllbase = AppDomain.CurrentDomain.BaseDirectory;


                                                        if (!string.IsNullOrEmpty(s.TypeLib))
                                                        {
                                                            var dllfile = dllbase + s.TypeLib;
                                                            if (APP.DynamicReferenceDlls.Contains(dllfile))
                                                            {
                                                                isref = false;
                                                            }
                                                            else
                                                            {
                                                                APP.DynamicReferenceDlls.Add(dllfile);
                                                                isref = true;
                                                                dlls.Add(dllfile);
                                                            }
                                                        }
                                                        if (!string.IsNullOrEmpty(s.ReferenceDlls))
                                                        {
                                                            isref = true;
                                                            string[] dllnams = s.ReferenceDlls.Split(',');
                                                            foreach (var n in dllnams)
                                                            {
                                                                APP.DynamicReferenceDlls.Add(dllbase + n);
                                                            }

                                                            dlls.AddRange(dllnams);
                                                        }
                                                        if (isref)
                                                        {
                                                            so_default = so_default.WithReferences(dlls.ToArray());
                                                        }
                                                        var state = CSharpScript.Create<object>(s.ProxyCode, so_default, typeof(Dictionary<string, object>));
                                                        //foreach (var submission in runcode.CodeLines)
                                                        //{
                                                        //    state = state.ContinueWithAsync(submission).Result;
                                                        //}
                                                        runcode.Script = state;
                                                        APP.RuntimeCodes.Add(s.Guid, runcode);
                                                    }
                                                    if (!string.IsNullOrEmpty(s.ReferenceDlls))
                                                    {
                                                        string dllbase = AppDomain.CurrentDomain.BaseDirectory;

                                                    }
                                                    rebject = runcode.Script.RunAsync(newrunmodel.ResouceInfos).Result.ReturnValue;

                                                }
                                            }
                                            break;
                                        case FlowStepType.GetData:
                                            {
                                                var ormname = config.GetValue<string>("App", "UsingDBConfig", "OrmName");
                                                DynaObject = HandleGetData(newrunmodel, dbFactory, ormname, s );
                                            }
                                            break;
                                        case FlowStepType.CallMethod:
                                            {
                                                var methodsub = APP.GetMethodFromConfig(s.IsBuildIn.Value, s.TypeLib, s.TypeFullName, s.MethodName);
                                                var objParams = new List<object>();
                                                if (!string.IsNullOrEmpty(s.ArgNames))
                                                {
                                                    objParams = newrunmodel.GetPoolResuce(s.ArgNames.Split(','));
                                                }

                                                else
                                                {
                                                    objParams = null;
                                                }
                                                try
                                                {
                                                    if (methodsub.IsStatic)
                                                    {

                                                        DynaObject = methodsub.Invoke(null, objParams.ToArray());
                                                    }
                                                    else
                                                    {
                                                        var instance = newrunmodel.ResouceInfos[s.InstanceName];
                                                        DynaObject = methodsub.Invoke(instance, objParams.ToArray());
                                                    }
                                                }
                                                catch (Exception exp)
                                                {
                                                    Logger.LogError(logName, "Run -> FlowStepType.CallMethod error,composity:{0},step:{1}", cons.Id, s.Guid);
                                                    break;
                                                }

                                            }
                                            break;
                                        case FlowStepType.Text:
                                            {
                                                rebject = s.OutPutText;
                                            }
                                            break;
                                        case FlowStepType.Function:
                                            {
                                                //var code = spec.ProxyCode;
                                                //var proxyMethode = CSharpScript.Create(code);
                                                //proxyMethode.RunAsync()
                                                //proxyMethode(objParams.ToArray());
                                            }
                                            break;
                                        case FlowStepType.RazorText:
                                            try
                                            {
                                                var engine = APP.Razorengine;
                                                string template = s.ProxyCode;
                                                if (string.IsNullOrEmpty(template))
                                                {
                                                    stepResult = "";
                                                }
                                                else
                                                {
                                                    var isok=  engine.Options.Namespaces.Add("UniOrm");
                                                    isok = engine.Options.Namespaces.Add("UniOrm.Application");
                                                    isok = engine.Options.Namespaces.Add("UniOrm.Common");
                                                    isok = engine.Options.Namespaces.Add("UniOrm.Model");
                                                    isok = engine.Options.Namespaces.Add("UniOrm.Startup.Web");

                                                    isok = engine.Options.Namespaces.Add("System");
                                                    isok = engine.Options.Namespaces.Add("System.Web");
                                                    isok = engine.Options.Namespaces.Add("System.IO");
                                                    isok = engine.Options.Namespaces.Add("System.Text");
                                                    isok = engine.Options.Namespaces.Add("System.Text.Encodings");
                                                    isok = engine.Options.Namespaces.Add("System.Text.RegularExpressions");
                                                    isok = engine.Options.Namespaces.Add("System.Collections.Generic");
                                                    isok = engine.Options.Namespaces.Add("System.Diagnostics");
                                                    isok = engine.Options.Namespaces.Add("System.Linq");
                                                    isok = engine.Options.Namespaces.Add("System.Security.Claims");
                                                    isok = engine.Options.Namespaces.Add("System.Threading");
                                                    isok = engine.Options.Namespaces.Add("System.Threading.Tasks");
                                                    isok = engine.Options.Namespaces.Add("System.Reflection");
                                                    isok = engine.Options.Namespaces.Add("System.Dynamic"); 
                                                    isok = engine.Options.Namespaces.Add("System.Diagnostics");
                                                    isok = engine.Options.Namespaces.Add("System.Web.Mvc.ViewPage");
                                                    isok = engine.Options.Namespaces.Add("System.Linq.Expressions");
                                                    isok = engine.Options.Namespaces.Add("System.Xml");
                                                    isok = engine.Options.Namespaces.Add("System.Xml.Linq");
                                                    isok = engine.Options.Namespaces.Add("System.Configuration");

                                                    isok = engine.Options.Namespaces.Add("System.Data");
                                                    isok = engine.Options.Namespaces.Add("System.Data.SqlClient");
                                                    isok = engine.Options.Namespaces.Add("System.Data.Common");
                                                    isok = engine.Options.Namespaces.Add("System.Data.OleDb");
                                                    isok = engine.Options.Namespaces.Add("System.Globalization");
                                                    isok = engine.Options.Namespaces.Add("System.Net");
                                                    isok = engine.Options.Namespaces.Add("System.Net.Http");
                                                    isok = engine.Options.Namespaces.Add("System.Net.Http.Headers");
                                                    isok = engine.Options.Namespaces.Add("System.Net.Mail");
                                                    isok = engine.Options.Namespaces.Add("System.Net.Security");
                                                    isok = engine.Options.Namespaces.Add("System.Net.Sockets");
                                                    isok = engine.Options.Namespaces.Add("System.Net.WebSockets");
                                                    isok = engine.Options.Namespaces.Add("System.Drawing");
                                                    isok = engine.Options.Namespaces.Add("System.Drawing.Printing");
                                                    isok = engine.Options.Namespaces.Add("Newtonsoft.Json");
                                                    isok = engine.Options.Namespaces.Add("Newtonsoft.Json.Linq");
                                                    isok = engine.Options.Namespaces.Add("System.Numerics"); 
                                                    isok = engine.Options.Namespaces.Add("Microsoft.AspNetCore.Authentication");
                                                    isok = engine.Options.Namespaces.Add("Microsoft.AspNetCore.Authorization");
                                                    isok = engine.Options.Namespaces.Add("Microsoft.Extensions.DependencyInjection");
                                                    isok = engine.Options.Namespaces.Add("Microsoft.AspNetCore.Mvc");
                                                  
                                                 
                                                    if (!string.IsNullOrEmpty( s.ReferenceDlls))
                                                    {
                                                        string[] dllnams = s.ReferenceDlls.Split(',');
                                                        foreach (var n in dllnams)
                                                        {
                                                            isok = engine.Options.Namespaces.Add(n.TrimEnd(".dll".ToCharArray())); 
                                                        } 
                                                    }
                                                    var objParams = newrunmodel.GetPoolResuce(s.ArgNames.Split(','));
                                                    var modelArg = objParams[0];
                                                    var cachekey2 = template.DesEncrypt().SafeSubString(128) + "_" + modelArg.ToJson().DesEncrypt().SafeSubString(64);
                                                    var cacheResult = engine.TemplateCache.RetrieveTemplate(cachekey2);
                                                    if (cacheResult.Success)
                                                    {
                                                        stepResult = await engine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), modelArg);
                                                    }
                                                    else
                                                    {
                                                        stepResult = await engine.CompileRenderAsync(cachekey2, template, modelArg);
                                                    }

                                                }

                                                rebject = stepResult;

                                            } 
                                            catch(Exception exp)
                                            {
                                                Logger.LogError(logName, "parser RazorText wrong: " + exp.Message + "-------" + LoggerHelper.GetExceptionString(exp));
                                            }
                                            break;
                                    }
                                    break;
                            }
                            if (rebject == null)
                            {
                                rebject = MagicExtension.BackToInst(DynaObject);

                            }
                            APP.RuntimeCache.Set(cacheKey, rebject);
                        }
                        else
                        {
                            rebject = stepResult;
                        }
                        if (!string.IsNullOrEmpty(s.StorePoolKey) && rebject != null)
                        {
                            newrunmodel.SetComposityResourceValue(s.StorePoolKey, rebject);
                        }

                    }
                    await CheckAndRunNextRuntimeComposity(requsetHash, newrunmodel, dbFactory, codeService, config);
                }

                //Manager.RuntimeModels.Remove(newrunmodel);
            }
        }

        private static object HandleGetData(RuntimeModel newrunmodel, IDbFactory dbFactory, string ormname, AConFlowStep s )
        {
            object DynaObject;
            //var ormname = config.GetValue<string>("App", "UsingDBConfig", "OrmName");

            var cacheormnameKey = string.Concat(s.InParamter1, "_", s.Connectionstring);
            object dbgrouder = APP.RuntimeCache.GetOrCreate(cacheormnameKey, entry =>
            {
                var isusingsystem = s.IsUsingParentConnstring;
                object dbgroudernew = null;
                if (!isusingsystem.HasValue || (isusingsystem.HasValue && isusingsystem.Value == true))
                {
                    dbgroudernew = newrunmodel.OpenDBSession(dbFactory, ormname, null);
                }
                else
                {
                    dbgroudernew = newrunmodel.OpenDBSession(dbFactory, ormname, s.Connectionstring);
                }
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

            var objParams2 = new List<object>();
            if (!string.IsNullOrEmpty(s.ArgNames))
            {
                var args = s.ArgNames.Split(',');
                foreach (var aarg in args)
                {
                    object obj = aarg;
                    if (aarg.StartsWith("&"))
                    {
                        obj = newrunmodel.Resuce(aarg);

                    }
                    objParams2.Add(obj);
                }
            }
            if (objParams2 == null || objParams2.Count == 0)
            {
                DynaObject = APP.GetData(dbgrouder, s.InParamter1);
            }
            else
            {
                DynaObject = APP.GetData(dbgrouder, s.InParamter1, objParams2.ToArray());
            }

            return DynaObject;
        }

        private static async Task CheckAndRunNextRuntimeComposity(int requsetHash, RuntimeModel newrunmodel, IDbFactory dbFactory, ICodeService codeService, IConfig config)
        {
            var resouce = newrunmodel.Resuce(newrunmodel.NextRunTimeKey);
            if (resouce != null)
            {
                var guid = (string)resouce;
                if (string.IsNullOrEmpty(guid))
                {
                    //newrunmodel.ResouceInfos.Remove(newrunmodel.NextRunTimeKey);
                    return;
                }
                var nextcon = codeService.GetConposity(guid).FirstOrDefault();
                if (nextcon == null)
                {
                    nextcon = new ComposeEntity()
                    {
                        Guid = guid,
                        RunMode = RunMode.Coding
                    };
                    var reint = codeService.InsertCode(nextcon);

                }
                else
                {
                    var nextRnmodel = new RuntimeModel(config)
                    {
                        ParentRuntimeModel = newrunmodel,
                        //ResouceInfos= newrunmodel.ResouceInfos,
                        ComposeEntity = nextcon,
                        HashCode = nextcon.GetHash()
                    };
                    //nextRnmodel.ResouceInfos.Remove(newrunmodel.NextRunTimeKey);
                   await  RunComposity(requsetHash, nextRnmodel, dbFactory, codeService, config);

                }
            }
        }

        #region Utilies




        private ComposeEntity FindComposity(AppConfig appconfig, string allname)
        {
            lock (lockobj)
            {
                ComposeEntity cons = APP.Composeentitys.FirstOrDefault(p => p.Guid == appconfig.StartUpCompoistyID);
                if (cons == null)
                {
                    cons = CodeService.GetConposity(appconfig.StartUpCompoistyID, allname).FirstOrDefault();
                    if (cons != null)
                    {
                        APP.Composeentitys.Add(cons);
                    }
                }

                if (cons == null)
                {
                    cons = new ComposeEntity()
                    {
                        Name = allname,
                        RunMode = RunMode.Coding
                    };
                    var reint = CodeService.InsertCode(cons);
                    APP.Composeentitys.Add(cons);
                }

                return cons;
            }
        }
        private ComposeTemplate FindComposeTemplet(string tid)
        {
            lock (lockobj)
            {
                var cons = APP.ComposeTemplates.FirstOrDefault(p => p.Guid == tid);
                if (cons == null)
                {
                    cons = CodeService.GetSimpleCodeLinq<ComposeTemplate>(p => p.Guid == tid).FirstOrDefault();
                    if (cons != null)
                    {
                        APP.ComposeTemplates.Add(cons);
                    }
                }


                return cons;
            }
        }
        private static IEnumerable<AConFlowStep> FindSteps(string ComId, ICodeService codeService)
        {

            var cons = APP.AConFlowSteps.Where(p => p.AComposityId == ComId);
            if (cons == null || cons.Count() == 0)
            {
                cons = codeService.GetAConStateSteps(ComId).OrderBy(p => p.StepOrder).ToList();
                if (cons != null)
                {
                    APP.AConFlowSteps.AddRange(cons);
                }
            }

            if (cons == null)
            {
                Logger.LogError(logName, "FindStep -> FindStep null");

            }

            return cons;

        }

        public TypeDefinition G(string typeName)
        {
            return CodeService.GetTypeDefinition(typeName);
        }
        #endregion
    }
}
