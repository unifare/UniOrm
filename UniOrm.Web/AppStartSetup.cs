using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UniOrm.StartUp;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.Extensions.Configuration;
using UniOrm.Common;
using Newtonsoft.Json.Serialization;
using UniOrm;

namespace AConState.Web
{
    public static class AppStartSetup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public static IServiceProvider ConfigureServices(this IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
            services.AddMvc(o =>
            {
                //o.Filters.Add<GlobalActionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var we = services.InitAutofac(null);
            InitDbMigrate();
            return we;
        }

        public static void InitDbMigrate()
        {
            AConStateStartUp.EnsureDaContext();
        }
        public static void ConfigureSite(this IApplicationBuilder app, IHostingEnvironment env)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            var webroot = Path.Combine(Directory.GetCurrentDirectory(), "webroot");
            if( !Directory.Exists( webroot))
            {
                Directory.CreateDirectory(webroot);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(webroot),
                RequestPath = ""
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = ""
            });
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                   "factory", "/fact/{action}", new { controller = "Fact", action = "Index" });

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                   "all", @"{**path}", new { controller = "Home", action = "Index" });

            });

        }
    }
}
