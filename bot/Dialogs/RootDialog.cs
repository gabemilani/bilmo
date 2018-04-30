using Financial.Bot.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Financial.Bot.Dialogs
{
    [Serializable]
    public sealed class RootDialog : IDialog<object>
    {
        public const string Error = "Error";
        public const string Ok = "Ok";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Type == ActivityTypes.Message)
            {
                if (context.UserRegistrationIsCompleted())
                {
                    await context.Forward(new FinancialDialog(), FinancialDialogCallback, activity, CancellationToken.None);
                }
                else
                {
                    await context.Forward(new WelcomeDialog(), WelcomeDialogCallback, activity, CancellationToken.None);
                }
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate && activity.MembersAdded?.Any(m => m.Id != activity.Recipient.Id) == true)
            {
                var name = context.GetUserName();
                var message = string.IsNullOrEmpty(name) ? "Olá, tudo bom?" : $"Olá, {name}!";
                await context.PostAsync(message);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task FinancialDialogCallback(IDialogContext context, IAwaitable<object> result)
        {
            await result;
            context.Wait(MessageReceivedAsync);
        }

        private async Task WelcomeDialogCallback(IDialogContext context, IAwaitable<object> result)
        {
            var obj = await result as string;

            if (string.Equals(obj, Ok))
            {
                context.FinishUserRegistration();
                await context.PostAsync("Se você tiver alguma dúvida é só me pedir ajuda!");
            }
            else if (string.Equals(obj, Error))
            {
                var msg = context.MakeMessage();
                msg.Type = ActivityTypes.EndOfConversation;
                await context.PostAsync(msg);
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}