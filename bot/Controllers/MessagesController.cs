using Financial.Bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Financial.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public MessagesController()
        {
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate && activity.MembersAdded?.Any(m => m.Id != activity.Recipient.Id) == true)
            {
                await Conversation.SendAsync(activity, () => new RootDialog());
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}