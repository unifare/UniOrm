using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UniOrm.Startup.Web.Authorize
{
    public interface IAuthorizeHelper
    {

        Task<TokenResponse> LoginToIds4Async(string usename, string password, string refreshToken = null);

        Task<TokenResponse> LoginToIds4Async(OauthClientModel clientmodel, string refreshToken = null);

        Task Logout(HttpContext context);


    }
}
