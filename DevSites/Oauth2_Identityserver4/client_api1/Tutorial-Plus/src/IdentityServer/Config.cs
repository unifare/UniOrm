// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
                 {
                    // 如果要请求 OIDC 预设的 scope 就必须要加上 OpenId(),
                    // 加上他表示这个是一个 OIDC 协议的请求
                    // Profile Address Phone Email 全部是属于 OIDC 预设的 scope
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResources.Phone(),
                    new IdentityResources.Email()
                 };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
         {
            // client credentials flow client
            new Client
            {
                ClientId = "mvc_client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                RedirectUris = { "http://localhost:5002/signin-oidc" },

                FrontChannelLogoutUri = "http://localhost:5002/signout-oidc",

                PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
            
                // 设置UserClaims添加到idToken中，而不是client需要重新使用用户端点去请求
                AlwaysIncludeUserClaimsInIdToken = true,
            
                // 允许离线访问，指是否可以申请 offline_access，刷新用的 token
                AllowOfflineAccess = true,

                AllowedScopes = new List<string>
                {
                    "api1",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Email,
                }
            }
        }; 
        }
    }
}