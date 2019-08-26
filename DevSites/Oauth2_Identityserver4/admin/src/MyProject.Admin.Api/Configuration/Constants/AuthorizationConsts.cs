namespace MyProject.Admin.Api.Configuration.Constants
{
    public class AuthorizationConsts
    {
        public const string IdentityServerBaseUrl = "http://localhost:6000";
        public const string OidcSwaggerUIClientId = "MyClientId_api_swaggerui";
        public const string OidcApiName = "MyClientId_api";

        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRole = "MyRole";
    }
}