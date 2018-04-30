using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace Financial.Api.Infrastructure.Filters
{
    public class BasicAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                var authenticationString = actionContext.Request.Headers.Authorization.Parameter;
                var originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

                var usrename = originalString.Split(':')[0];
                var password = originalString.Split(':')[1];

                if (!ValidUser(usrename, password))
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            base.OnAuthorization(actionContext);
        }

        private static bool ValidUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            if (!string.Equals(username, Settings.BasicAuthUsername))
                return false;

            if (!string.Equals(password, Settings.BasicAuthPassword))
                return false;

            return true;
        }
    }
}