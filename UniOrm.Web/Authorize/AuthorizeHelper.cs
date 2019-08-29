using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using UniOrm.Common;

namespace UniOrm.Startup.Web.Authorize
{
    public class AuthorizeHelper:IAuthorizeHelper
    {
        HttpContext httpContext = null;
         AppConfig AppConfig { get; set; }
        public AuthorizeHelper(IConfig config)
        {
            AppConfig = config.GetValue<AppConfig>("App");
        }
        public async Task<TokenResponse> LoginToIds4Async(string usename, string password, string refreshToken=null)
        {
            var identityserver4url = AppConfig.GetDicstring("Identityserver4.url");
            var ClientId = AppConfig.GetDicstring("idsr4_ClientId") ;
            var ClientSecret = AppConfig.GetDicstring("idsr4_ClientSecret");
            var clientmodel = new OauthClientModel()
            {
                IdentityUrl = identityserver4url,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                UserName = usename,
                Password = password

            };
            return await LoginToIds4Async( clientmodel,  refreshToken);

        }

        public async Task<TokenResponse> LoginToIds4Async(OauthClientModel clientmodel,  string refreshToken = null)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
                        { Address = clientmodel.IdentityUrl, Policy = { RequireHttps = false } } );
           //Token作废
            //var oldaccesstoken= wait client.RevokeTokenAsync(new TokenRevocationRequest() { Token = "" });
            if (refreshToken != null)
            {
                var RequesttokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = clientmodel.ClientSecret,
                    ClientSecret = clientmodel.ClientSecret,
                    Scope = clientmodel.Scope,
                    GrantType = OpenIdConnectGrantTypes.RefreshToken,
                    RefreshToken = refreshToken,
                });

                if (RequesttokenResponse.IsError)
                {
                    var tokenResponse = await LoginGetAccesstoken(client, clientmodel.ClientId,
                        clientmodel.ClientSecret, clientmodel.UserName, clientmodel.Password,
                        disco.TokenEndpoint);
                    return tokenResponse;
                }
                else
                {
                    var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(RequesttokenResponse.ExpiresIn);

                    var tokens = new[] {
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.IdToken,
                            Value = RequesttokenResponse.IdentityToken
                        },
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.AccessToken,
                            Value = RequesttokenResponse.AccessToken
                        },
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.RefreshToken,
                            Value = RequesttokenResponse.RefreshToken
                            },
                            new AuthenticationToken
                            {
                                Name = "expires_at",
                                Value = expiresAt.ToString("o",CultureInfo.InvariantCulture)
                            }
                        };
                    // 获取身份认证的结果，包含当前的pricipal和 properties
                    var currentAuthenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // 把新的tokens存起来
                    currentAuthenticateResult.Properties.StoreTokens(tokens);

                    // 登陆
                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);
                    //HttpContext.Response.Cookies.Append(["refreshToken"]. = tokenResponse.RefreshToken;

                    return RequesttokenResponse;
                }
            }
            else
            {
                var tokenResponse = await LoginGetAccesstoken(client, clientmodel.ClientId,
                    clientmodel.ClientSecret, clientmodel.UserName, clientmodel.Password,
                    disco.TokenEndpoint);

                return tokenResponse;

            }

        }

  
        public async Task<Response> Logout(string token)
        {
            var identityserver4url = AppConfig.GetDicstring("Identityserver4.url");
            var ClientId = AppConfig.GetDicstring("idsr4_ClientId");
            var ClientSecret = AppConfig.GetDicstring("idsr4_ClientSecret");
            var client = new HttpClient();

            JwtSecurityToken jwt = new JwtSecurityToken(token);
             
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            { Address = identityserver4url, Policy = { RequireHttps = false } });



            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = "http://oauth.66wave.com/connect/introspect",
                ClientId = ClientId,
                ClientSecret = ClientSecret,

                Token = token
            });
            //client.vil
            var RequesttokenResponse = await client.RevokeTokenAsync(new  TokenRevocationRequest()
            {
                Address = disco.RevocationEndpoint,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Token = token
            });
            return RequesttokenResponse;
        }


        private async Task<Response> RequestUserInfo(string endpoint_url, HttpClient client, string accessToken)
        {
            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = endpoint_url,
                Token = accessToken
            });
            return response;


        }
        private async Task<TokenResponse> LoginGetAccesstoken(HttpClient client, string clientid,
            string ClientSecret, string UserName, string Password,
            string TokenEndpoint)
        {
            var ptr = new PasswordTokenRequest();
            ptr.Address = TokenEndpoint;
            ptr.ClientId = clientid;
            ptr.ClientSecret = ClientSecret;
            ptr.UserName = UserName;
            ptr.Password = Password;

            var tokenResponse = await client.RequestPasswordTokenAsync(ptr);

            return tokenResponse;
        }

    }
}
