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
using Autofac;
using Microsoft.IdentityModel.Tokens;
using UniOrm.Core;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UniOrm.Startup.Web
{
    public static class WebSetup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public static IServiceProvider ConfigureServices(this IServiceCollection services)
        {
            //register configer
            JsonConfig jsonConfig = new JsonConfig();
            var pa = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config\\system.json");
            jsonConfig.Source = File.ReadAllText(pa);
            var builder = new ContainerBuilder();
            builder.RegisterInstance<IConfig>(jsonConfig);
            var tempcontainer = builder.Build();

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
            // 配置授权
            var config = tempcontainer.Resolve<IConfig>();
            var appConfig = config.GetValue<AppConfig>("App");
            var signingkey = GetDicstring(appConfig, "JWT.IssuerSigningKey");
            var backendfoldername = GetDicstring(appConfig, "backend.foldername"); 
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            } )
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
               {
                   o.LoginPath = new PathString("/Account/Login");
                   o.AccessDeniedPath = new PathString("/Error/Forbidden");
               })
               .AddCookie(UserAuthorizeAttribute.CustomerAuthenticationScheme, option =>
               {
                   option.LoginPath = new PathString("/Account/Login");
                   option.AccessDeniedPath = new PathString("/Error/Forbidden");
               })
                   .AddCookie(AdminAuthorizeAttribute.CustomerAuthenticationScheme, option =>
                   {
                       option.LoginPath = new PathString("/" + backendfoldername + "/Admin/Signin");
                       option.AccessDeniedPath = new PathString("/Error/Forbidden");
                   })
                   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
            (jwtBearerOptions) =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingkey)),//秘钥
                    ValidateIssuer = true,
                    ValidIssuer = GetDicstring(appConfig,"JWT.Issuer"),
                    ValidateAudience = true,
                    ValidAudience = GetDicstring(appConfig,"JWT.Audience"),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
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
        public static string GetDicstring(AppConfig appConfig, string key)
        {
            var item = appConfig.SystemDictionaries.FirstOrDefault(p => p.KeyName == key);
            if (item != null)
            {
                return item.Value;
            }
            return string.Empty;
        }
        public static void InitDbMigrate()
        {
            ApplicationStartUp.EnsureDaContext();
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

            app.UseAuthentication();//配置授权
                                    //处理异常
            app.UseStatusCodePages(new StatusCodePagesOptions()
            {
                HandleAsync = (context) =>
                {
                    if (context.HttpContext.Response.StatusCode == 401)
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(context.HttpContext.Response.Body))
                        {
                            sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                status = 401,
                                message = "access denied!",
                            }));
                        }
                    }
                    return System.Threading.Tasks.Task.Delay(0);
                }
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
