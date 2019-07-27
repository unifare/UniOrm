using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniOrm.Loggers;

namespace AConState.Web
{
    public class ControllerActionFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Logger.LogInfo("GlobalActionFilter", "全局ActionFilter执行之前");
            await next();
            Logger.LogInfo("GlobalActionFilter", "全局ActionFilter执行之前");
        }
    }
}
