using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using UniOrm;
using UniOrm.Model;
using UniOrm.Model.DataService;
using UniOrm.Startup.Web;

namespace UniNote.WebClient.Controllers
{
    [Area("sd23nj")]
    [AdminAuthorize] 
    public class DbFormController : Controller
    {
        IHostingEnvironment _hostingEnvironment;
        ISysDatabaseService m_codeService;
        public DbFormController(ISysDatabaseService codeService, IHostingEnvironment hostingEnvironment)
        {

            m_codeService = codeService;
            _hostingEnvironment = hostingEnvironment;

            //var ss = HttpContext.Session["admin"] ?? "";
            //if( )
        }


        public IActionResult Index()
        {
            return   View();
        }


    }
}
