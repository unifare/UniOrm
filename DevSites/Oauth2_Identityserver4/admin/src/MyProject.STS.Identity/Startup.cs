using AspNet.Security.OpenId;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyProject.Admin.EntityFramework.Shared.DbContexts;
using MyProject.Admin.EntityFramework.Shared.Entities.Identity;
using MyProject.STS.Identity.Helpers;
using System;

namespace MyProject.STS.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public ILogger Logger { get; set; }

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
            Environment = environment;
            Logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureRootConfiguration(Configuration);

            // Add DbContext for Asp.Net Core Identity
            services.AddIdentityDbContext<AdminIdentityDbContext>(Configuration, Environment);

            // Add email senders which is currently setup for SendGrid and SMTP
            services.AddEmailSenders(Configuration);

            // Add services for authentication, including Identity model, IdentityServer4 and external providers
            services.AddAuthenticationServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, UserIdentity, UserIdentityRole>(Environment, Configuration, Logger);
            services.AddAuthentication()
              .AddGoogle(options =>
              {
                  options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:6000/signin-google
                    options.ClientId = "copy client ID from Google here";
                  options.ClientSecret = "copy client secret from Google here";
              }) 
              .AddOpenId("Orange", "Orange", options =>
               {
                   options.Authority = new Uri("https://openid.orange.fr/");
                   options.CallbackPath = "/signin-orange";
               })

            .AddOpenId("StackExchange", "StackExchange", options =>
            {
                options.Authority = new Uri("https://openid.stackexchange.com/");
                options.CallbackPath = "/signin-stackexchange";
            })

            .AddOpenId("Intuit", "Intuit", options =>
            {
                options.CallbackPath = "/signin-intuit";
                options.Configuration = new OpenIdAuthenticationConfiguration
                {
                    AuthenticationEndpoint = "https://openid.intuit.com/OpenId/Provider"
                };
            })

            .AddSteam();
            // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
            // Including settings for MVC and Localization
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddMvcWithLocalization<UserIdentity, string>();

            // Add authorization policies for MVC
            services.AddAuthorizationPolicies();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.AddLogging(loggerFactory, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add custom security headers
            app.UseSecurityHeaders();

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcLocalizationServices();
            app.UseMvcWithDefaultRoute();
        }
    }
}
