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
using System.IO;
using Microsoft.AspNetCore.Http;

namespace UniNote.WebClient.Controllers
{
    public class FileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RelatedPath { get; set; }
        public string ParentDirName { get; set; }

        public string FileExtense { get; set; }
        public bool IsDirectry { get; set; }
        public bool IsReaderable { get; set; }
        public bool IsWritable { get; set; }
        public bool IsTextFile { get; set; }
        public string LinuxRights { get; set; }

    }


    [Area("sd23nj")]
    [AdminAuthorize]
    public class AFileController : Controller
    {
        IDbFactory dbFactory;
        public AFileController(IDbFactory _dbFactory)
        {
            dbFactory = _dbFactory;
        }
        public IActionResult GetAllFiles(string dir)
        {
            var basedir = AppDomain.CurrentDomain.BaseDirectory;
            var fullpath = dir.ToServerFullPath();
            if (!Directory.Exists(fullpath))
            {
                return new JsonResult(new { isok = false });
            }
            else
            {
                var index = 0;
                var list = new List<FileDTO>();
                var padir = fullpath.GetDirFullPath();
                if (dir != "/")
                {
                    if (padir.Replace("\\", "").Replace("/", "") == basedir.Replace("\\", "").Replace("/", ""))
                    {
                        padir = "/";
                    }
                    else
                    {
                        padir = fullpath.GetDirName();
                    }
                    list.Add(new FileDTO { Id = index, Name = "..上一级", RelatedPath = padir.FindAndSubstring(basedir).Replace("\\", "/"), ParentDirName = padir, IsDirectry = true });
                    index++;
                }

                var allfiles = Directory.GetFiles(fullpath);
                var subdirs = Directory.GetDirectories(fullpath);
                //var allobjects = subdirs.Concat(allfiles); 
                foreach (var item in subdirs)
                {
                    var fileio = new DirectoryInfo(item);

                    list.Add(new FileDTO()
                    {
                        Id = index,
                        RelatedPath = fileio.FullName.FindAndSubstring(basedir).Replace("\\", "/"),
                        Name = fileio.Name,
                        FileExtense = fileio.Extension,
                        ParentDirName = dir,
                        IsDirectry = true,
                        IsReaderable = false,
                        IsWritable = true
                    });
                    index++;
                }
                foreach (var item in allfiles)
                {
                    var fileio = new FileInfo(item);

                    list.Add(new FileDTO()
                    {
                        Id = index,
                        RelatedPath = fileio.FullName.FindAndSubstring(basedir).Replace("\\", "/"),
                        Name = fileio.Name,
                        IsTextFile = fileio.FullName.IsTextFile(),
                        FileExtense = fileio.Extension,
                        ParentDirName = dir,
                        IsDirectry = false,
                        IsReaderable = fileio.IsReadOnly,
                        IsWritable = !fileio.IsReadOnly
                    });
                    index++;
                }
                return new JsonResult(new { isok = true, parentdir = dir, list = list });
            }

        }
        public IActionResult GetFileContent(string rePath)
        {
            rePath = rePath.UrlDecode();
            var fullpath = rePath.ToServerFullPath();
            var content = fullpath.ReadAsTextFile();

            return new JsonResult(new { isok = true, content });
        }

        public IActionResult CreateFile(string dirname, string name)
        {
            var basedir = AppDomain.CurrentDomain.BaseDirectory.CombineFilePath(dirname);

            basedir.EnSureDirectroy();
            var file = basedir.CombineFilePath(name);
            var isCreated = file.FileToCreated();
            return new JsonResult(new { isok = isCreated });
        }

        public IActionResult SaveFile(string path, string context)
        {
            var sw = path.OpenTextFileReadyWrite();
            sw.Write(context);
            sw.Close();
            return new JsonResult(new { isok = true });
        }

        public IActionResult RenameFile(string path, string newfilename)
        {
            var isok = path.FileRename(newfilename);
            return new JsonResult(new { isok });
        }

        public IActionResult DelFile(string path)
        {
            return new JsonResult(new { isok = path.FileDelete() });
        }

        public IActionResult UploadFile(string dirName)
        {
            var remsg = string.Empty;
            if (Request.Form.Files.Count == 0)
            {
                remsg = ("未检测到文件");
                return new JsonResult(new { isok = false, msg = remsg });
            }
            dirName.ToServerFullPathEnEnsure();

            foreach (var file in Request.Form.Files)
            {
                file.UploadSaveSingleFile(dirName);
            }
            return new JsonResult(new { isok = true, msg = remsg });
        }


        public IActionResult FileMng()
        {
            return View();
        }

    }
}
