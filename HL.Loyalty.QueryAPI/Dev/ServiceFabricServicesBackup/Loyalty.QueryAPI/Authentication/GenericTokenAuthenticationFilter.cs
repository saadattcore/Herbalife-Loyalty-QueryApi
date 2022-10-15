using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Loyalty.QueryAPI.Authentication
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class GenericTokenAuthenticationFilter : AuthorizationFilterAttribute
    {
        private readonly bool _isActive = true;
        private string[] _roles = null;

        public GenericTokenAuthenticationFilter(params string[] roles)
        {
            _roles = roles;
        }

        public GenericTokenAuthenticationFilter(bool isActive)
        {
            _isActive = isActive;
        }

        /// <summary>
        /// It will check if token is available in http request header
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!_isActive) return;

            if (actionContext.Request.Headers.Contains("Token"))
            {
                var token = actionContext.Request.Headers.GetValues("Token").FirstOrDefault();

                if (!String.IsNullOrEmpty(token) && !IsValidToken(token)) // ToDo write token authentication code
                {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("UnAuthorize")
                    };
                }
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("No token found")
                };
            }


            base.OnAuthorization(actionContext);
        }

        /// <summary>
        /// Validate auth token passed in request header.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsValidToken(string token)
        {
            // TODO - Implement token verification logic.           

            var authToken = HL.Common.Configuration.Settings.GetRequiredAppSetting("AuthToken");

            return authToken.Equals(token);
        }

    }
}