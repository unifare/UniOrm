using UniOrm.Common; 
using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm.Core
{
    public class AppConfig
    {
        public string AppType { get; set; }

        public DcConnectionConfig UsingDBConfig { get; set; }
        public List<DcConnectionConfig> Connectionstrings { get; set; }
        public List<RegestedModel> EFRegestedModels { get; set; }
        public List<string> OrmTypes { get; set; }
        public string TrigerType { get; set; }
        public string StartUpCompoistyID { get; set; }
        // 
    }


}
