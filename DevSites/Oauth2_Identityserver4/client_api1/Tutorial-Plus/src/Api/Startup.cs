using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api
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
            var urls = new string[] { "http://localhost:7000", "http://localhost:6000" };
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin", builder =>
                {
                    builder.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetPreflightMaxAge(TimeSpan.FromSeconds(1728000));
                });
            });
            services.AddMvcCore().AddAuthorization().AddJsonFormatters();
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:6000"; // IdentityServer的地址
                    options.RequireHttpsMetadata = false; // 不需要Https

                    options.Audience = "api1"; // 和资源名称相对应
                                               // 多长时间来验证以下 Token
                    options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);
                    // 我们要求 Token 需要有超时时间这个参数
                    options.TokenValidationParameters.RequireExpirationTime = true;

                })

            //.AddIdentityServerAuthentication(options =>
            //{
            //    options.Authority = "http://localhost:6000";
            //    options.RequireHttpsMetadata = false;

            //    options.ApiName = "api1";
            //    options.ApiSecret = "api1pwd";  //对应ApiResources中的密钥
            //});
            ;

        
        } 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowAllOrigin");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
