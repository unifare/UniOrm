using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm.Startup.Web.Controllers
{
    public class FactoryBuilderController : Controller
    {
        public IActionResult Index()
        {
            //var ss = dbFactory.EFCore<pigcms_adma>().CreateDefaultInstance();

            //var qlist = ss.From<pigcms_adma>();
            //var allist = qlist.ToList<pigcms_adma>();
            return new EmptyResult();
        }
    }
}
