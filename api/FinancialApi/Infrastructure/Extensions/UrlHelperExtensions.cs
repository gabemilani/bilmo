using System.Web.Http.Routing;

namespace Financial.Api.Infrastructure.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string LinkController(this UrlHelper urlHelper, object controller, object id = null)
        {
            if (id == null)
                return urlHelper.Link("DefaultApi", new { controller });

            return urlHelper.Link("DefaultApi", new {  controller,  id });
        }
    }
}