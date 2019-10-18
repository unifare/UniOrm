using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace UniOrm
{
    public class RazorTool
    {
        public HttpContext HttpContext { get; set; }
        public RazorTool()
        {
            IHttpContextAccessor factory = APP.ApplicationServices.GetService<IHttpContextAccessor>();
            HttpContext = factory.HttpContext;
        }

        public dynamic GetData(string sql, object[] args)
        {
            return APP.GetData(sql, args);
        }

        public string Session(string key)
        {
            return HttpContext.Session.GetString(key);
        }

        public void Session(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

       


        public string UrlQuery(string key)
        {
            return HttpContext.Request.Query[key];
        }

        public string GetForm(string key)
        {
            return HttpContext.Request.Form[key];
        }

        public HttpRequest Request
        {
            get
            {
                return HttpContext.Request;
            }
        }

        public HttpResponse Respone
        {
            get
            {
                return HttpContext.Response;
            }
        }
        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        public void SetCookies(string key, string value, int minutes = 30)
        {
            HttpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }
        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        public void DeleteCookies(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        public string GetCookies(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }
    }
}
