using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static IdentityModel.OidcConstants;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // 关闭Jwt的Claim类型映射，以便允许 well-known claims (e.g. ‘sub’ and ‘idp’) 
            // 如果不关闭就会修改从授权服务器返回的 Claim
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
                options.Authority = "http://localhost:6000";
                // 表示我们不需要 Https
                options.RequireHttpsMetadata = false;
                // 用于在cookie中保留来自IdentityServer的 token，因为以后可能会用
                options.SaveTokens = true; 
                options.ClientId = "mvc_client";
                options.ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A";
                options.ResponseType = "code"; // Authorization Code

                options.Scope.Clear();
                options.Scope.Add("api1");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("address");
                options.Scope.Add("phone");
                options.Scope.Add("email");
                // Scope中添加了OfflineAccess后，就可以在 Action 中获得 refreshToken
                options.Scope.Add(StandardScopes.OfflineAccess);
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // 管道中加入身份验证功能
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
