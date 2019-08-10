using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm.Application.ModuleStander
{
    public class DefaultModule : IModule
    {
        public string ModuleName { get; set; }
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public RequireItemCollection RequireItems { get; set; }

        public List<IGodWorker> GodWorkers { get; set; }

        public DefaultModule()
        {
            Types = new Dictionary<string, Type>();
        }

        //public virtual object GetObject(string objectName)
        //{

        //}
        //public object GetMethod(string objectName);
    }
}
