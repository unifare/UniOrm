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
using static IdentityModel.OidcConstants;
using UniOrm.Loggers;
using MediatR;
using System.Threading.Tasks;
using NetCoreCMS.Framework.Core.App;
using Microsoft.AspNetCore;
using System.Threading;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace UniOrm.Startup.Web
{
    public static class WebSetup
    {
        private static IWebHost nccWebHost;
        private static Thread starterThread = new Thread(StartApp);
        private static AppConfig appConfig = GodWorker.appConfig;
        private const string LoggerName = "WebSetup";



        public static void StartApp(object argsObj)
        {
            BuildWebHost((string[])argsObj).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {

            nccWebHost = WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
                 .Build();
            //nccWebHost = WebHost.CreateDefaultBuilder(args)
            //    .UseKestrel(c => c.AddServerHeader = false)
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseIISIntegration()
            //    .UseStartup<Startup>()
            //    .UseDefaultServiceProvider(options => options.ValidateScopes = false)
            //    //.UseApplicationInsights()
            //    .Build();
            return nccWebHost;
        }

        public static async Task RestartAppAsync()
        {
            await NetCoreCmsHost.StopAppAsync(nccWebHost);
        }

        public static async Task ShutdownAppAsync()
        {
            await NetCoreCmsHost.ShutdownAppAsync(nccWebHost);
        }
        public static void Startup(this IConfiguration configuration)
        {
            APP.Startup(configuration);
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public static IServiceProvider ConfigureServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMediatR(typeof(WebSetup).Assembly);
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
            services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromSeconds(60 * 60);
            });

            // 配置授权
            var config = tempcontainer.Resolve<IConfig>();
            GodWorker.appConfig = appConfig = config.GetValue<AppConfig>("App");
            var signingkey = GetDicstring(appConfig, "JWT.IssuerSigningKey");
            var backendfoldername = GetDicstring(appConfig, "backend.foldername");
            var AuthorizeCookiesName = GetDicstring(appConfig, "AuthorizeCookiesName");
            var OdicCookiesName = GetDicstring(appConfig, "OdicCookiesName");
            var identityserver4url = GetDicstring(appConfig, "Identityserver4.url");
            var Identityserver4ApiResouceKey = GetDicstring(appConfig, "Identityserver4.ApiResouceKey");
            var idsr4_ClientId = GetDicstring(appConfig, "idsr4_ClientId");
            var idsr4_ClientSecret = GetDicstring(appConfig, "idsr4_ClientSecret");
            var idsr4_ReponseType = GetDicstring(appConfig, "idsr4_ReponseType");
            var OauthClientConfig_scopes = GetDicstring(appConfig, "OauthClientConfig_scopes");
            var IsUsingIdentityserverClient = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingIdentityserverClient"));
            var IsUsingIdentityserver4 = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingIdentityserver4"));
            var isAllowCros = Convert.ToBoolean(GetDicstring(appConfig, "isAllowCros"));
            var AllowCrosUrl = GetDicstring(appConfig, "AllowCrosUrl");
            var IsUserAutoUpdatedb = Convert.ToBoolean(GetDicstring(appConfig, "IsUserAutoUpdatedb"));
            var isEnableSwagger = Convert.ToBoolean(GetDicstring(appConfig, "isEnableSwagger")); ;
            //services.AddMvcCore().AddAuthorization().AddJsonFormatters(); 
            var IsUsingLocalIndentity = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingLocalIndentity"));
            // var IsUsingDB = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingDB"));
            var IsUsingCmsGlobalRouterFilter = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingCmsGlobalRouterFilter"));



            if (isEnableSwagger)
            {
                //注册Swagger生成器，定义一个和多个Swagger 文档
                services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new Info
                            {
                                Version = "v1",
                                Title = " API",
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
            }

            if (IsUsingLocalIndentity)
            {
                services.AddAuthentication(
                    options =>
                {
                    if (IsUsingIdentityserverClient == false || IsUsingIdentityserver4 == false)
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                }
                )
                 .AddCookie(UserAuthorizeAttribute.CustomerAuthenticationScheme, option =>
                {
                    option.LoginPath = new PathString("/account/login");
                    option.AccessDeniedPath = new PathString("/Error/Forbidden");
                })
                .AddCookie(AdminAuthorizeAttribute.CustomerAuthenticationScheme, option =>
                     {
                         option.LoginPath = new PathString("/" + backendfoldername + "/Admin/Signin");
                         option.AccessDeniedPath = new PathString("/Error/Forbidden");
                     })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        if (IsUsingIdentityserver4)
                        {
                            //options.JwtValidationClockSkew = TimeSpan.FromSeconds(0);
                            options.Authority = identityserver4url; // IdentityServer的地址
                            options.RequireHttpsMetadata = false; // 不需要Https 
                            options.Audience = Identityserver4ApiResouceKey; // 和资源名称相对应 
                        }
                        else
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingkey)),//秘钥
                                ValidateIssuer = true,
                                ValidIssuer = GetDicstring(appConfig, "JWT.Issuer"),
                                ValidateAudience = true,
                                ValidAudience = GetDicstring(appConfig, "JWT.Audience"),
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.FromMinutes(5)
                            };
                        }
                        options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);
                        // 我们要求 Token 需要有超时时间这个参数
                        options.TokenValidationParameters.RequireExpirationTime = true;
                        //};
                    });
            }
            if (IsUsingIdentityserver4 && !IsUsingLocalIndentity)
            {
                services.AddMvcCore().AddAuthorization().AddJsonFormatters();
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                   {
                       options.Authority = identityserver4url; // IdentityServer的地址
                       options.RequireHttpsMetadata = false; // 不需要Https 
                       options.Audience = Identityserver4ApiResouceKey; // 和资源名称相对应 
                       options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);
                       options.TokenValidationParameters.RequireExpirationTime = true;
                   });

            }
            if (IsUsingIdentityserverClient)
            {
                services.AddAuthentication(options =>
                 {
                     // 使用cookie来本地登录用户（通过DefaultScheme = "Cookies"）
                     options.DefaultScheme = AuthorizeCookiesName;
                     // 设置 DefaultChallengeScheme = "oidc" 时，表示我们使用 OIDC 协议
                     options.DefaultChallengeScheme = OdicCookiesName;
                 })
                    // 我们使用添加可处理cookie的处理程序
                    .AddCookie(AuthorizeCookiesName)
                    // 配置执行OpenID Connect协议的处理程序

                    .AddOpenIdConnect(OdicCookiesName, options =>
                    {
                        // 
                        options.SignInScheme = AuthorizeCookiesName;
                        // 表明我们信任IdentityServer客户端
                        options.Authority = identityserver4url;
                        // 表示我们不需要 Https
                        options.RequireHttpsMetadata = false;
                        // 用于在cookie中保留来自IdentityServer的 token，因为以后可能会用
                        options.SaveTokens = true;
                        try
                        {
                            options.ClientId = idsr4_ClientId; // "mvc_client";
                            options.ClientSecret = idsr4_ClientSecret;
                            options.ResponseType = idsr4_ReponseType;
                        }
                        catch (Exception exp)
                        {
                            Logger.LogError(LoggerName, "exp: " + exp.Message + ",------------->" + LoggerHelper.GetExceptionString(exp));
                        }
                        options.Scope.Clear();
                        var allscopes = OauthClientConfig_scopes.Split(',');
                        foreach (var ss in allscopes)
                        {
                            options.Scope.Add(ss);
                        }

                    })
                 ;

            }
            if (isAllowCros)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("allow_all", bb =>
                    {

                        if (AllowCrosUrl == "*")
                        {
                            bb = bb.AllowAnyOrigin();
                        }
                        else
                        {
                            var allusrs = AllowCrosUrl.Split(',');
                            bb = bb.WithOrigins(allusrs);
                        }

                        bb.AllowAnyMethod()
                         .AllowAnyHeader()
                         .AllowCredentials();//指定处理cookie
                    });
                });
            }

            services.AddMvc(o =>
            {
                if (IsUsingCmsGlobalRouterFilter)
                {
                    o.Filters.Add<GlobalActionFilter>();
                }
            })
            .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); })
           .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var asses = AppDomain.CurrentDomain.GetAssemblies();
            var we = services.InitAutofac(asses);
            APP.Container.Resolve<IConfig>().GetValue<AppConfig>().ResultDictionary = appConfig.ResultDictionary;
            if (IsUserAutoUpdatedb)
            {
                InitDbMigrate();
            }

            APP.InitDbMigrate();
            APP.ConfigureSiteAllModulesServices(services);
            APP.ApplicationServices = services.BuildServiceProvider();
            APP.SetServiceProvider();

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
            Application.DbMigrationHelper.EnsureDaContext(APPCommon.AppConfig.UsingDBConfig);
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
            var UserDefaultStaticalDir = GetDicstring(appConfig, "UserDefaultStaticalDir");
            var webroot = Path.Combine(Directory.GetCurrentDirectory(), UserDefaultStaticalDir);
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

            APP.ConfigureSiteAllModules(app);

            app.UseCookiePolicy(); //是否启用cookie隐私

            var isEnableSwagger = Convert.ToBoolean(GetDicstring(appConfig, "isEnableSwagger")); ;
            if (isEnableSwagger)
            {
                //启用中间件服务生成Swagger作为JSON终结点
                app.UseSwagger();
                //启用中间件服务对swagger-ui，指定Swagger JSON终结点
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            app.UseAuthentication();
            //配置授权
            //处理异常
            //app.UseStatusCodePages(new StatusCodePagesOptions()
            //{
            //    //HandleAsync = (context) =>
            //    //{
            //    //    //if (context.HttpContext.Response.StatusCode == 401)
            //    //    //{
            //    //    //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(context.HttpContext.Response.Body))
            //    //    //    {
            //    //    //        sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
            //    //    //        {
            //    //    //            status = 401,
            //    //    //            message = "access denied!",
            //    //    //        }));
            //    //    //    }
            //    //    //}
            //    //    //return System.Threading.Tasks.Task.Delay(0);
            //    //}
            //});
            var isAllowCros = Convert.ToBoolean(GetDicstring(appConfig, "isAllowCros"));
            if (isAllowCros)
            {
                app.UseCors("allow_all");
            }
            var AreaName = GetDicstring(appConfig, "AreaName");

            //url 重写。。。
            // var rewrite = new RewriteOptions()
            //.AddRewrite("first", "home/index", 301);
            // app.UseRewriter(rewrite);

            //用户session服务
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                 "areas",
                    "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                //routes.MapRoute("areaRoute", "{"+ AreaName + ":exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                   "factory", "/fact/{action}", new { controller = "Fact", action = "Index" });
                routes.MapRoute(
                "cmsinstall", "/CmsInstall/{action}", new { controller = "CmsInstall", action = "Index" });

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                   "all", @"{**path}", new { controller = "FactoryBuilder", action = "Index" });

            });

        }
    }
}
