using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UniOrm.Application;

namespace UniOrm.Application
{
    public enum ModuleName
    {
        Security,
        WebUtility,
        Other
    }
   
    public class DefaultModuleManager
    {
        public static ModuleCollection RegistedModules { get; set; }

        public static Guid Guid { get; set; }
        public DefaultModuleManager()
        {
            Guid = new Guid();
        }
        public static RequireItemCollection TotalRequireItems
        {
            get
            {
                var cachemodel = SuperManager.RuntimeCache.GetOrCreate(Guid, entry =>
               {
                   RequireItemCollection totalRequireItems = new RequireItemCollection();
                   foreach (var m in RegistedModules)
                   {
                       foreach (var mc in m.RequireItems)
                       {
                           if (totalRequireItems.FirstOrDefault(prop => prop.Name == mc.Name && prop.version == mc.version) != null)
                           {
                               totalRequireItems.Add(mc);
                           }

                       } 
                   }
                   SuperManager.RuntimeCache.Set(Guid, totalRequireItems);
                   return totalRequireItems;
               });

                return cachemodel;
            }
        }

        public static IModule GetModule(ModuleName name, string othername = null)
        {
            return RegistedModules.FirstOrDefault(p => p.ModuleName == name.ToString());
            //loger
        }
        public static IModule AddModule(IModule module)
        {
            RegistedModules.Add(module);
            return module;
        }
    }
}
