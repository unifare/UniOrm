using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MvcClient.Models;
using Newtonsoft.Json.Linq;

namespace MvcClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:6000/");
            if (disco.IsError)
                throw new Exception(disco.Error);

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var id_token = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("http://localhost:6008/api/values");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await RenewTokensAsync();
                    return RedirectToAction();
                }
                throw new Exception(response.ReasonPhrase);
            }

            //ViewData["content"] = await response.Content.ReadAsStringAsync();

            return View();
        }
        private async Task<string> RenewTokensAsync()
        {
            // 得到发现文档
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:6000/");
            if (disco.IsError)
                throw new Exception(disco.Error);
            //var tokenClient = new TokenClient(disco.TokenEndpoint, "mvc", "744726ef-2f8f-ca39-ef42-e92a976ed4e0");
            // 得到 RefreshToken 
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            //刷新 Access Token
            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "mvc",
                ClientSecret = "744726ef-2f8f-ca39-ef42-e92a976ed4e0",
                Scope = $"api1 UniNote_WebApi openid profile offline_access",
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                RefreshToken = refreshToken,
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
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

                return tokenResponse.AccessToken;
            }
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(); // MVC 登出
            await HttpContext.SignOutAsync("oidc"); // IdentityServer4 登出
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
