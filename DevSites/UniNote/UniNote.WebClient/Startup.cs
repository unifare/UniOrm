using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
using UniOrm.Application;
using UniOrm.Startup.Web;
using static IdentityModel.OidcConstants;

namespace UniNote
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var serv = services.ConfigureServices();

            //ApplicationStartUp.EnsureDaContext(typeof(MigrationVersion1).Assembly);
            return serv;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ConfigureSite(env);
        }
    }
}
