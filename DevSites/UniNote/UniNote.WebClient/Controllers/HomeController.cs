﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniNote.Web.Model;
using UniNote.WebClient.Models;
using UniOrm;

namespace UniNote.WebClient.Controllers
{ 
 
    public class HomeController : Controller
    {
        IDbFactory dbFactory;
        public HomeController(IDbFactory _dbFactory)
        {
           dbFactory = _dbFactory;
        }
        public IActionResult Index2()
        {
           var ss = dbFactory.EFCore<pigcms_adma>().CreateDefaultInstance();

            //var qlist = ss.From<pigcms_adma>();
            //var allist = qlist.ToList<pigcms_adma>();
            return View();
        }
       
        public IActionResult odic_login()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public object test(string test)
        {
            return new { ok = test } ;
        }
        public IActionResult UserWelcome()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
