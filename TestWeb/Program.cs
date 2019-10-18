using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreCMS.Framework.Core.App;
using UniOrm.Startup.Web;

namespace TestWeb
{
    public class Program
    {

        public static void Main(string[] args)
        {
            // BuildWebHost(args).Run();
            WebSetup.StartApp(args);


        }

      
    }
}
