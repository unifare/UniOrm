 
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace AConState.Web
{
    //public class GlobalActionFilter : IAsyncActionFilter
    //{
    //    ITypeMaker TypeMaker;
    //    public GlobalActionFilter(ITypeMaker typeMaker)
    //    {
    //        TypeMaker = typeMaker;
    //    }
    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        //var factory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
    //        // var s = context.HttpContext.Request.Path;
    //        //var logger = factory.CreateLogger<GlobalActionFilter>();
    //        await ExcuteFilter(context, next);
    //        // logger.LogWarning("全局ActionFilter执行之后");
    //    }
    //    public static string regtext = @"\.(css|ico|jpg|png|gif|bmp|js)+\?*.*$";
    //    public static Regex reg = new Regex(regtext);

    //    private async Task ExcuteFilter(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        var repath = context.HttpContext.Request.Path.Value.ToLower();
          
    //        if (repath.StartsWith("/api/fact") || reg.IsMatch(repath))
    //        {
    //            await next();

    //        }
    //        else
    //        {
    //            TypeMaker.Run(context);
    //            await next();
    //        }
    //        //var resp = context.HttpContext.Response;
    //        //resp.ContentType = "text/html";

    //        //using (StreamWriter sw = new StreamWriter(resp.Body))
    //        //{
    //        //    sw.Write("Write a string to response in WriteResponseWithoutReturn!");
    //        //}
    //    }
    //}
}
