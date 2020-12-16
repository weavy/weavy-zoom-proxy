using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;

namespace Weavy.Zoom.Proxy.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]

    public class TokenAuthorizationAttribute : TypeFilterAttribute
    {
        public TokenAuthorizationAttribute() : base(typeof(AuthorizeActionFilter))
        {
        }
    }

    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public AuthorizeActionFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // get the token in the Authorization header. This is the secret token specified in the Zoom app
            var authHeaderToken = context.HttpContext.Request.Headers["Authorization"];

            // get the token from the appsettings.json. This should match the token in the Authorization header
            var verificationToken = _configuration["weavy.zoom-webhook-verification-token"];

            if (!authHeaderToken.Equals(verificationToken))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
