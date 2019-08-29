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

namespace UniOrm.Startup.Web
{
    public static class WebSetup
    {
        private static AppConfig appConfig = null;
        private const string LoggerName = "WebSetup";
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


            // 配置授权
            var config = tempcontainer.Resolve<IConfig>();
            appConfig = config.GetValue<AppConfig>("App");
            var signingkey = GetDicstring(appConfig, "JWT.IssuerSigningKey");
            var backendfoldername = GetDicstring(appConfig, "backend.foldername");
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
            var isEnableSwagger = Convert.ToBoolean(GetDicstring(appConfig, "isEnableSwagger")); ;
            //services.AddMvcCore().AddAuthorization().AddJsonFormatters(); 
            var IsUsingLocalIndentity = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingLocalIndentity"));
            // var IsUsingDB = Convert.ToBoolean(GetDicstring(appConfig, "IsUsingDB"));

            if (isEnableSwagger)
            {
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
                     options.DefaultScheme = "Cookies";
                     // 设置 DefaultChallengeScheme = "oidc" 时，表示我们使用 OIDC 协议
                     options.DefaultChallengeScheme = "oidc";
                 })
                    // 我们使用添加可处理cookie的处理程序
                    .AddCookie("Cookies")
                    // 配置执行OpenID Connect协议的处理程序

                    .AddOpenIdConnect("oidc", options =>
                    {
                        // 
                        options.SignInScheme = "Cookies";
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
                o.Filters.Add<GlobalActionFilter>();
            })
            .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); })
           .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var asses = AppDomain.CurrentDomain.GetAssemblies();
            var we = services.InitAutofac(asses);
            SuperManager.Container.Resolve<IConfig>().GetValue<AppConfig>().ResultDictionary = appConfig.ResultDictionary;
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
            app.UseAuthentication();//配置授权
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
            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                   "factory", "/fact/{action}", new { controller = "Fact", action = "Index" });

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
                //routes.MapRoute(
                //   "all", @"{**path}", new { controller = "Home", action = "Index" });

            });

        }
    }
}
