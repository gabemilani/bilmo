using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Web;
using System.Web.Http;

namespace Financial.Bot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            HttpContext.Current.Server.ScriptTimeout = 300;
            Conversation.UpdateContainer(builder => builder.RegisterModule(new ReflectionSurrogateModule()));
        }
    }
}