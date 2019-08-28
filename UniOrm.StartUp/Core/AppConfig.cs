using UniOrm.Common;
using System;
using System.Collections.Generic;
using System.Text;
using UniOrm.Model;
using System.Linq;

namespace UniOrm
{
    public class AppConfig
    {
        private AppConfig()
        {
            SystemDictionaries = new List<SystemDictionary>();
            ResultDictionary = new Dictionary<string, object>(); 
        }
        public string AppType { get; set; }

        public DcConnectionConfig UsingDBConfig { get; set; }
        public List<DcConnectionConfig> Connectionstrings { get; set; }
        public List<RegestedModel> EFRegestedModels { get; set; }
        public List<string> OrmTypes { get; set; }
        public string TrigerType { get; set; }
        public string StartUpCompoistyID { get; set; }
        public List<SystemDictionary> SystemDictionaries { get; set; }
        public Dictionary<string, object> ResultDictionary { get; set; }
        // 

        public string GetDicstring(string key)
        {
            var item = SystemDictionaries.FirstOrDefault(p => p.KeyName == key);
            if (item != null)
            {
                return item.Value;
            }
            return string.Empty;
        }
    }


}
