using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOrm;
using UniOrm.Application;
using UniOrm.Startup.Web;
using UniOrm.Model;
using UniOrm.Model.DataService;

namespace UniNote.WebClient.Controllers
{
    [Area("sd23nj")]
    [AdminAuthorize]
    public class DictionaryController : Controller
    {
        ISysDatabaseService m_codeService;
        public DictionaryController(ISysDatabaseService codeService)
        {
            m_codeService = codeService;
        }

        public IActionResult Index()
        {
            return View();
        }




        public IActionResult VueGrid2()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Signin()
        {
            return View();
        }

        public IActionResult SaveKeyvalue(string key, string value)
        {
            //var sd = new SystemDictionary();// { AddTime = DateTime.Now, IsSystem = false, KeyName = key, Value = value, SystemDictionarytype = SystemDictionarytype.String };
            var recode = m_codeService.GetSimpleCode<SystemDictionary>(new { KeyName = key }).FirstOrDefault();
            if (recode == null)
            {
                recode = new SystemDictionary() { AddTime = DateTime.Now, IsSystem = false, KeyName = key, Value = value, SystemDictionarytype = SystemDictionarytype.String };

                var result = m_codeService.InsertCode<SystemDictionary>(recode);
            }
            else
            {
                recode.Value = value;
                m_codeService.UpdateSimpleCode(recode);
            }

            return new JsonResult(new { result = recode });
        }
        public IActionResult DelKey(string key)
        {
            var result = true;
            //var sd = new SystemDictionary();// { AddTime = DateTime.Now, IsSystem = false, KeyName = key, Value = value, SystemDictionarytype = SystemDictionarytype.String };
            var recode = m_codeService.GetSimpleCode<SystemDictionary>(new { KeyName = key }).FirstOrDefault();
            if (recode != null)
            {
                result = m_codeService.DeleteSimpleCode(recode);
            } 
            return new JsonResult(new { isok = result });
        }


        public IActionResult GetAll(string key, int pIndex, int pagesize)
        {

            var result = m_codeService.GetSimpleCodePage<SystemDictionary>(null, pIndex, pagesize);
            return new JsonResult(new { result = result });
            //var basedir = AppDomain.CurrentDomain.BaseDirectory;
            //var fullpath = dir.ToServerFullPath();
            //if (!Directory.Exists(fullpath))
            //{
            //    return new JsonResult(new { isok = false });
            //}
            //else
            //{
            //    var index = 0;
            //    var list = new List<FileDTO>();
            //    var fullpadir = fullpath.GetDirFullPath();
            //    if (dir != "/")
            //    {
            //        var padir = fullpadir;
            //        if (padir.Replace("\\", "").Replace("/", "") == basedir.Replace("\\", "").Replace("/", ""))
            //        {
            //            padir = "/";
            //        }
            //        else
            //        {
            //            padir = fullpath.GetDirName();
            //        }
            //        list.Add(new FileDTO { Id = index, Name = "..上一级", RelatedPath = fullpadir.FindAndSubstring(basedir).Replace("\\", "/"), ParentDirName = padir, IsDirectry = true });
            //        index++;
            //    }

            //    var allfiles = Directory.GetFiles(fullpath);
            //    var subdirs = Directory.GetDirectories(fullpath);
            //    //var allobjects = subdirs.Concat(allfiles); 
            //    foreach (var item in subdirs)
            //    {
            //        var fileio = new DirectoryInfo(item);

            //        list.Add(new FileDTO()
            //        {
            //            Id = index,
            //            RelatedPath = fileio.FullName.FindAndSubstring(basedir).Replace("\\", "/"),
            //            Name = fileio.Name,
            //            FileExtense = fileio.Extension,
            //            ParentDirName = dir,
            //            IsDirectry = true,
            //            IsReaderable = false,
            //            IsWritable = true
            //        });
            //        index++;
            //    }
            //    foreach (var item in allfiles)
            //    {
            //        var fileio = new FileInfo(item);

            //        list.Add(new FileDTO()
            //        {
            //            Id = index,
            //            RelatedPath = fileio.FullName.FindAndSubstring(basedir).Replace("\\", "/"),
            //            Name = fileio.Name,
            //            IsTextFile = fileio.FullName.IsTextFile(),
            //            FileExtense = fileio.Extension,
            //            ParentDirName = dir,
            //            IsZipFile = fileio.Extension.ToLower().Contains("zip") || fileio.Extension.ToLower().Contains("raa") || fileio.Extension.ToLower().Contains("7z") || fileio.Extension.ToLower().Contains("tar"),
            //            IsDirectry = false,
            //            IsReaderable = fileio.IsReadOnly,
            //            IsWritable = !fileio.IsReadOnly
            //        });
            //        index++;
            //    }
            //    return new JsonResult(new { isok = true, parentdir = dir, list = list });
            //}

        }
        //[HttpGet]
        //[HttpPost]
        //public async Task<RedirectResult> GetAll()
        //{
        //    await HttpContext.SignOutAsync(AdminAuthorizeAttribute.CustomerAuthenticationScheme); 
        //    return Redirect("~/");
        //}



    }
}
