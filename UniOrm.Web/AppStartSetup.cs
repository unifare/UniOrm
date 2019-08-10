using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.Extensions.Configuration;
using UniOrm.Common;
using Newtonsoft.Json.Serialization;
using UniOrm;
using UniOrm.Application;
using Microsoft.AspNetCore.Authentication.Cookies;
using Swashbuckle.AspNetCore.Swagger;

namespace UniOrm.Startup.Web
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
            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "yilezhu's API",
                    Description = "A simple example ASP.NET Core Web API",
                    Contact = new Contact
                    {
                        Name = "Oliver Wa",
                        Email = string.Empty,
                        Url = "http://www.66wave.com/"
                    },
                    License = new License
                    {
                        Name = "许可证名字",
                        Url = "http://www.66wave.com/"
                    }
                });
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
               {
                   o.LoginPath = new PathString("/Account/Login");
                   o.AccessDeniedPath = new PathString("/Error/Forbidden");
               })
                   .AddCookie(CustomerAuthorizeAttribute.CustomerAuthenticationScheme, option =>
                   {
                       option.LoginPath = new PathString("/sdfsdf/Admin/Signin");
                       option.AccessDeniedPath = new PathString("/Error/Forbidden");
                   });
            services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
            services.AddMvc(o =>
            {
               o.Filters.Add<GlobalActionFilter>();
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
            if (!Directory.Exists(webroot))
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
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");
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
