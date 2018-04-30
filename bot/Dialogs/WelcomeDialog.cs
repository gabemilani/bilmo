using Financial.Bot.Extensions;
using Financial.Bot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financial.Bot.Dialogs
{
    [Serializable]
    public sealed class WelcomeDialog : IDialog<object>
    {
        private const string Done = "Welcome";
        private const string Yes = "Claro!";
        private const string No = "Talvez outra hora";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;            

            await activity.StartTypingAndWaitAsync();

            var registrationComplete = await EnsureUserRegistrationCompleted(context);
            if (registrationComplete)
            {
                await context.PostAsync($"Olá, {context.GetUserName()}! Alguma novidade?");
                context.Done(RootDialog.Ok);                
            }
            else
            {
                var id = context.GetUserId();
                if (id.HasValue)
                {
                    await context.PostAsync($"Olá, {context.GetUserName()}!");
                    await RegisterUserWalletAsync(context, activity);
                }
                else
                {
                    await WelcomeNewMemberAsync(context, activity);
                }
            }
        }

        private async Task<bool> EnsureUserRegistrationCompleted(IDialogContext context)
        {
            var id = context.GetUserId();
            if (id.HasValue)
            {
                var user = await FinancialApi.Dispatcher.GetUserAsync(id.Value);
                if (user != null)
                {
                    context.SetUserName(user.Name);

                    var walletId = context.GetUserWalletId();
                    if (walletId.HasValue)
                    {
                        var wallet = await FinancialApi.Dispatcher.GetWalletAsync(walletId.Value);
                        if (wallet != null)
                            return true;

                        context.RemoveUserWalletId();
                    }

                    var userWallets = await FinancialApi.Dispatcher.GetUserWalletsAsync(user.Id);
                    var userWallet = userWallets.FirstOrDefault();
                    if (userWallet != null)
                    {
                        context.SetUserWalletId(userWallet.Id);
                        return true;
                    }
                }                
            }

            context.RemoveUserId();
            context.RemoveUserName();
            context.RemoveUserWalletId();

            return false;
        }

        private async Task WelcomeNewMemberAsync(IDialogContext context, Activity activity)
        {
            if (context.UserReadedFirstSteps())
            {
                await context.PostAsync("Espero que tenha mudado de ideia!");
            }
            else
            {
                await context.PostAsync("Hey! Percebi que você é novo por aqui");

                await activity.StartTypingAndWaitAsync();
                await context.PostAsync("Deixa eu me apresentar para você!");

                await activity.StartTypingAndWaitAsync();
                await context.PostAsync("Eu sou o Bilmo!");

                await activity.StartTypingAndWaitAsync();
                await context.PostAsync("Eu ajudo as pessoas a controlarem suas finanças por mensagens! Legal, né?");

                context.FinishFirstSteps();
            }

            await activity.StartTypingAndWaitAsync();

            PromptDialog.Choice(
                context,
                OnOptionSelectedAsync,
                new Dictionary<string, IEnumerable<string>>
                {
                    { Yes, new [] { "Sim", "Quero", "Por favor", "Registrar", "Fazer parte", "Pode ser", "Por que não", "Yes", "Sim, por favor", "Agora mesmo"  } },
                    { No, new [] { "Não", "Não, obrigado", "Outra hora", "Agora não", "Melhor não", "Nem", "Na", "nao", } }
                },
                "Então, o que acha de fazer parte disso?",
                "Desculpa, não consegui entender o que você quis dizer!",
                3);
        }        

        private async Task OnOptionSelectedAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var value = await result;

                if (Equals(value, Yes))
                {
                    await context.PostAsync("Obaaa! Você não vai se arrepender! Vamos lá!");
                    await Task.Delay(1000);
                    await context.PostAsync("Como eu posso lhe chamar?");

                    context.Wait(CreateUserAsync);
                }
                else if (Equals(value, No))
                {
                    await context.PostAsync("Que pena! :( ");
                    await Task.Delay(1000);
                    await context.PostAsync("Estarei aqui se mudar de ideia!");

                    context.Done(RootDialog.Error);
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task CreateUserAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var name = activity.Text;

            await activity.StartTypingAndWaitAsync();
            await context.PostAsync($"É um prazer lhe conhecer, {name}!");
            await activity.StartTypingAndWaitAsync();

            CreateUserAsync(context, name);

            await RegisterUserWalletAsync(context, activity);
        }

        private async void CreateUserAsync(IDialogContext context, string name)
        {
            var userId = await FinancialApi.Dispatcher.CreateUserAsync(name);

            context.SetUserId(userId);
            context.SetUserName(name);
        }

        private async Task RegisterUserWalletAsync(IDialogContext context, Activity activity)
        {
            await context.PostAsync("Preciso cadastrar uma carteira para você, qual é o saldo atual dela?");

            await activity.StartTypingAndWaitAsync();
            await context.PostAsync("Pode ficar tranquilo que essa informação é sigilosa, ok?");

            context.Wait(CreateUserWalletAsync);
        }

        private async Task CreateUserWalletAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            await activity.StartTypingAndWaitAsync();

            var recognizes = new PromptRecognizer().RecognizeDouble(activity);
            var recognize = recognizes.FirstOrDefault();

            if (recognize != null)
            {
                var userId = context.GetUserId();
                var walletId = await FinancialApi.Dispatcher.CreateWalletAsync(userId.Value, recognize.Entity);
                context.SetUserWalletId(walletId);
                await context.PostAsync("Bacana! Temos tudo configurado agora! Podemos começar a registrar suas transações!");
                context.Done(RootDialog.Ok);
            }
            else
            {
                await context.PostAsync("Por favor, escreva um valor válido!");
                context.Wait(CreateUserWalletAsync);
            }
        }
    }
}