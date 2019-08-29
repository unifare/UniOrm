using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniNote.Web.Model;
using UniNote.WebClient.Models;
using UniOrm;

namespace UniNote.WebClient.Controllers
{ 
    
    public class AccountController : Controller
    {
        //IDbFactory dbFactory;
        public AccountController( )
        {
            //dbFactory = _dbFactory;
        }

        public async Task<RedirectResult> SignOut()
        {
            await HttpContext.SignOutAsync( );
            await HttpContext.SignOutAsync("oidc");
            return Redirect("~/");
        }
    
        public IActionResult Privacy()
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
