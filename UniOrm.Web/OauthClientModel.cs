using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm.Startup.Web
{
    public class OauthClientModel
    {
        public string IdentityUrl { get; set; }
        public string Endpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string GrantType { get; set; }
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
