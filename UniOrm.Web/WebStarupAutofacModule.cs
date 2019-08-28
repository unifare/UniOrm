using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using UniOrm.Startup.Web.Authorize;
namespace UniOrm.Startup.Web
{
    public class WebStarupAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthorizeHelper>().As<IAuthorizeHelper>();
            base.Load(builder);

        }
    }
}
