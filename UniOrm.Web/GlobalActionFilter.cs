using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UniOrm.Application;

namespace UniOrm.Startup.Web
{
    public class GlobalActionFilter : IAsyncActionFilter
    {
        IGodWorker TypeMaker;
        public GlobalActionFilter(IGodWorker typeMaker)
        {
            TypeMaker = typeMaker;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var factory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
            // var s = context.HttpContext.Request.Path;
            //var logger = factory.CreateLogger<GlobalActionFilter>();
            await ExcuteFilter(context, next);
            // logger.LogWarning("全局ActionFilter执行之后");
        }


        public static string regtext = @"\.(css|ico|jpg|png|gif|bmp|js)+\?*.*$";
        public static Regex reg = new Regex(regtext);

        private async Task ExcuteFilter(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var repath = context.HttpContext.Request.Path.Value.ToLower();

            if (repath.StartsWith("/sdfsdf/") || repath.StartsWith("/api/fact") || reg.IsMatch(repath))
            {
                await next();

            }
            else
            {
                await TypeMaker.Run(context);
                if (context.Result == null)
                {
                    await next();
                }

            }
            //var resp = context.HttpContext.Response;
            //resp.ContentType = "text/html";

            //using (StreamWriter sw = new StreamWriter(resp.Body))
            //{
            //    sw.Write("Write a string to response in WriteResponseWithoutReturn!");
            //}
        }
    }
}
