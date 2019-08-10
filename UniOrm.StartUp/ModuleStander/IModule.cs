using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm.Application
{
    public interface IModule
    {
        string ModuleName { get; set; }
        List<IGodWorker> GodWorkers { get; set; }
        RequireItemCollection RequireItems { get; set; }
    }
}
