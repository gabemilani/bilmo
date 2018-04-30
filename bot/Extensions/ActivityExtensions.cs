using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace Financial.Bot.Extensions
{
    public static class ActivityExtensions
    {
        public static async Task StartTypingAndWaitAsync(this Activity activity, int millisecondsToWait = 1500)
        {
            var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl));
            var typingReply = activity.CreateReply();
            typingReply.Type = ActivityTypes.Typing;

            await connectorClient.Conversations.ReplyToActivityAsync(typingReply);
            await Task.Delay(millisecondsToWait);
        }
    }
}