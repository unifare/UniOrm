using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniNote.DBMigration;
using UniOrm.Application;
using UniOrm.Startup.Web;

namespace UniNote
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            var serv = services.ConfigureServices();
            ApplicationStartUp.EnsureDaContext(typeof(MigrationVersion1).Assembly);
            return serv;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ConfigureSite(env);
        }
    }
}
