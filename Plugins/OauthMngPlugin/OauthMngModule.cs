﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using UniOrm;
using UniOrm.Common;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Autofac;
using static IdentityModel.OidcConstants;

namespace OauthMngPlugin
{
    public class OauthMngModule : ModuleBase
    {
        public override string ModuleName { get; } = nameof(OauthMngModule);
        public override string DllPath { get; set; }
        public override AppConfig ModuleAppConfig { get; set; }
        public override DbConnectionConfig dcConnectionConfig { get; set; }
        public OauthMngModule()
        {
        }
        public OauthClientModel OauthClient { get; set; }
        public override void EnsureDaContext()
        {
            UniOrm.Common.DbMigrationHelper.EnsureDaContext(
               dcConnectionConfig.Connectionstring,
               (int)APPCommon.AppConfig.UsingDBConfig.DBType,
               typeof(OauthMngModule).Assembly);
        }

        public override bool Init()
        {
            base.Init();
            return true;
        }
        public override void SetServiceProvider(ServiceProvider serviceProvider)
        {

            ServiceProvider = serviceProvider;
        }

        public override bool MigrateDB()
        {
            return true;
        }

        public override List<Type> ModelType()
        {
            throw new NotImplementedException();
        }

        public override List<Type> FunctionalTypes { get; set; }

        public override List<string> ModelTypeStrings()
        {
            return new List<string>();
        }

        public override List<Autofac.Module> GetAutofacModules()
        {
            return new List<Autofac.Module>();
        }

        public override void ConfigureSiteServices(IServiceCollection services)
        {
            //Builder.RegisterInstance<OauthMngPlugin.OauthMngModule>()

        }

        public override void RegisterAutofacTypes()
        {
            Builder.RegisterInstance<OauthMngPlugin.OauthMngModule>(this);

            OauthClient = new OauthClientModel();
            OauthClient.ClientId = Configuration["OauthMngModule:OidcSwaggerUIClientId"];
            OauthClient.IdentityUrl = Configuration["OauthMngModule:IdentityServerBaseUrl"];
            OauthClient.ReponseType = Configuration["OauthMngModule:idsr4_ReponseType"];
            OauthClient.IdentityUrl = Configuration["OauthMngModule:IdentityServerBaseUrl"];
            OauthClient.ReponseType = Configuration["OauthMngModule:idsr4_ReponseType"];
            OauthClient.ReponseType = Configuration["OauthMngModule:idsr4_ReponseType"];
            OauthClient.ClientSecret = Configuration["OauthMngModule:idsr4_ClientSecret"];
            OauthClient.Scope = Configuration["OauthMngModule:OauthClientConfig_scopes"];
            OauthClient.UserName = Configuration["OauthMngModule:idsr4_username"];
            OauthClient.Password = Configuration["OauthMngModule:idsr4_password"];
            OauthClient.GrantType = GrantTypes.Password;
            //OauthClient = Configuration["OauthMngModule:OidcApiName"];
        }
    }
}
