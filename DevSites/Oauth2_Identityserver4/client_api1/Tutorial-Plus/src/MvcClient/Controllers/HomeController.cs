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

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:6000/");
            if (disco.IsError)
                throw new Exception(disco.Error);

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("http://localhost:6001/api/values");
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
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies"); // MVC 登出
            await HttpContext.SignOutAsync("oidc"); // IdentityServer4 登出
        }
        // 当token失效，请求新的token
        private async Task<string> RenewTokensAsync()
        {
            // 得到发现文档
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:6000/");
            if (disco.IsError)
                throw new Exception(disco.Error);

            // 得到 RefreshToken 
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            //刷新 Access Token
            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "mvc client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = $"api1 openid profile address email phone",
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
        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // 想要获得 refreshToken 必须在MVC客户端的 Scope 单独添加 OfflineAccess
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            ViewData["accessToken"] = accessToken;
            ViewData["idToken"] = idToken;
            ViewData["refreshToken"] = refreshToken;

            return View();
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
