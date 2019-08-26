using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string userName, [FromForm] string password, [FromForm] string refreshToken)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:6000/");
            var name = userName;
            var pass = password;

            if (refreshToken != null)
            { 
                var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "mvc",
                    ClientSecret = "744726ef-2f8f-ca39-ef42-e92a976ed4e0",
                    Scope = $"api1 UniNote_WebApi openid profile offline_access role",
                    GrantType = OpenIdConnectGrantTypes.RefreshToken,
                    RefreshToken = refreshToken,
                });

                if (tokenResponse.IsError)
                {
                    tokenResponse = await RequestNewAccesstoken(client, disco.TokenEndpoint);
                    return new JsonResult(tokenResponse);
                }
                else
                {
                    var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);

                    var tokens = new[] {
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.IdToken,
                            Value = tokenResponse.IdentityToken
                        },
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.AccessToken,
                            Value = tokenResponse.AccessToken
                        },
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.RefreshToken,
                            Value = tokenResponse.RefreshToken
                            },
                            new AuthenticationToken
                            {
                                Name = "expires_at",
                                Value = expiresAt.ToString("o",CultureInfo.InvariantCulture)
                            }
                        };
                    // 获取身份认证的结果，包含当前的pricipal和 properties
                    var currentAuthenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // 把新的tokens存起来
                    currentAuthenticateResult.Properties.StoreTokens(tokens);

                    // 登陆
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);
                    //HttpContext.Response.Cookies.Append(["refreshToken"]. = tokenResponse.RefreshToken;
                    HttpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
                    {
                        Expires = expiresAt
                    });
                    HttpContext.Response.Cookies.Append("accessToken", tokenResponse.RefreshToken, new CookieOptions
                    {
                        Expires = expiresAt
                    });
                    return new JsonResult(tokenResponse);
                }
            }
            else
            {
                var tokenResponse = await RequestNewAccesstoken(client, disco.TokenEndpoint);

                return new JsonResult(tokenResponse);

            }

            // discover endpoints from metadata




        }

        private async Task<TokenResponse> RequestNewAccesstoken(HttpClient client, string Address)
        {
            var ptr = new PasswordTokenRequest();
            ptr.Address = Address;
            ptr.ClientId = "mvc";
            ptr.ClientSecret = "744726ef-2f8f-ca39-ef42-e92a976ed4e0";
            ptr.UserName = "bbbb";
            ptr.Password = "Okokspsp@123";

            var tokenResponse = await client.RequestPasswordTokenAsync(ptr);
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            HttpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
            {
                Expires = expiresAt
            });
            HttpContext.Response.Cookies.Append("accessToken", tokenResponse.RefreshToken, new CookieOptions
            {
                Expires = expiresAt
            });
            return tokenResponse;
        }
    }
}