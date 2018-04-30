using Financial.Bot.Extensions;
using Financial.Bot.Models;
using Financial.Bot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financial.Bot.Dialogs
{
    [Serializable]
    public sealed class FinancialDialog : LuisDialog<object>
    {
        private const string DateTimeEntity = "builtin.datetimeV2.daterange";
        private const string MoneyEntity = "builtin.currency";
        private const string NumberEntity = "builtin.number";        

        private readonly static TransactionCategory OtherCategory = new TransactionCategory { Name = "Outras" };

        public FinancialDialog()
            : base(new LuisService(
                new LuisModelAttribute(
                    modelID: AppSettings.LuisModelId,
                    subscriptionKey: AppSettings.LuisSubscriptionKey,
                    domain: AppSettings.LuisDomain)))
        {

        }

        [LuisIntent("Balance")]
        public async Task BalanceAsync(IDialogContext context, LuisResult result)
        {
            var walletId = context.GetUserWalletId();
            if (walletId.HasValue)
            {
                var wallet = await FinancialApi.Dispatcher.GetWalletAsync(walletId.Value);
                if (wallet != null)
                {
                    await context.PostAsync($"Saldo atual: R$ {wallet.CurrentBalance:0.00}");
                    context.Done(RootDialog.Ok);
                    return;
                }
            }

            await context.PostAsync("Você precisa cadastrar uma carteira para poder consultar o seu saldo");
            context.Done(RootDialog.Error);
        }

        [LuisIntent("Consciousness")]
        public async Task ConsciousnessAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Eu sou o robô Bilmo! Ajudo pessoas a controlarem suas finanças por mensagens!");
            context.Done(RootDialog.Ok);
        }

        [LuisIntent("Extract")]
        public async Task ExtractAsync(IDialogContext context, LuisResult result)
        {
            var walletId = context.GetUserWalletId();
            if (!walletId.HasValue)
            {
                await context.PostAsync("Você precisa cadastrar uma carteira para poder consultar o extrato");
                context.Done(RootDialog.Error);
                return;
            }
            
            Transaction[] transactions = null;

            var dateEntity = result.Entities.FirstOrDefault(e => string.Equals(e.Type, DateTimeEntity));
            if (dateEntity != null && 
                dateEntity.Resolution.TryGetValue("values", out var values) && 
                values is List<object> objects && 
                objects.FirstOrDefault() is Dictionary<string, object> dictionary &&
                dictionary.TryGetValue("start", out var start) &&
                dictionary.TryGetValue("end", out var end))
            {
                var fromDate = DateTime.Parse(start.ToString());
                var toDate = DateTime.Parse(end.ToString());

                await context.PostAsync($"Buscando transações que aconteceram entre {fromDate.ToString("dd/MM/yyy")} e {toDate.ToString("dd/MM/yyyy")}. Por favor, aguarde!");

                transactions = await FinancialApi.Dispatcher.GetTransactionsAsync(walletId.Value, fromDate, toDate);
            }
            else
            {
                int? transactionsCount = null;

                var numberEntities = result.Entities.Where(e => string.Equals(e.Type, NumberEntity));
                foreach (var numberEntity in numberEntities)
                {
                    if (numberEntity.Resolution.TryGetValue("value", out var resolutionValue) && int.TryParse(resolutionValue.ToString(), out var count))
                    {
                        transactionsCount = count;
                        break;
                    }
                }

                if (transactionsCount.HasValue)
                {
                    await context.PostAsync($"Buscando as últimas {transactionsCount} transações. Por favor, aguarde!");
                }
                else
                {
                    transactionsCount = 10;
                    await context.PostAsync($"Buscando as últimas transações. Por favor, aguarde!");
                }

                transactions = await FinancialApi.Dispatcher.GetTransactionsAsync(walletId.Value, transactionsCount.Value);
            }

            if (transactions?.Any() == true)
            {
                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                foreach (var transaction in transactions)
                {
                    var card = new HeroCard
                    {
                        Title = transaction.Type.GetDescription(),
                        Subtitle = transaction.CategoryName,
                        Text = string.Concat(
                                $"Data: {transaction.DateTime.ToString("dd/MM/yyyy")} \n\n",
                                $"Hora: {transaction.DateTime.ToString("HH:mm")} \n\n",
                                $"Valor: R$ {Math.Round(transaction.Value, 2):0.00} \n\n",
                                $"Descrição: \"{transaction.Description}\"")
                    };

                    var attachment = card.ToAttachment();
                    reply.Attachments.Add(attachment);
                }

                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("Nenhuma transação encontrada");
            }

            context.Done(RootDialog.Ok);
        }

        [LuisIntent("Greeting")]
        public async Task GreetingAsync(IDialogContext context, LuisResult result)
        {
            var name = context.GetUserName();
            await context.PostAsync($"Olá, {name}! Tudo bem?");
            context.Done(RootDialog.Ok);
        }

        [LuisIntent("Help")]
        public async Task HelpAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Eu posso ajudar você das seguintes formas: visualizar o seu saldo, visualizar o seu extrato e registrar suas transações!");
            await Task.Delay(1500);
            await context.PostAsync("Basta pedir para ver o seu saldo que eu mostro para você seu saldo atual!");
            await Task.Delay(1500);
            await context.PostAsync("Você pode pedir para ver o seu extrato também. Pode visualizar o extrato filtrando por data ou pelas últimas transações!");
            await Task.Delay(1500);
            await context.PostAsync("Para registrar suas transações comigo é só me falar o que você fez e quanto gastou! Vou citar alguns exemplos:");
            await Task.Delay(1500);
            await context.PostAsync("- Hey, Bilmo! Comprei um pastel aqui na universidade por 5 reais!");
            await Task.Delay(1500);
            await context.PostAsync("- Putz! Perdi 20 reais que estavam no meu bolso!");
            await Task.Delay(1500);
            await context.PostAsync("- Fulana, me pagou os 50 reais que emprestei para ela!");
            await Task.Delay(1500);
            await context.PostAsync("Depois disso, nós vamos categorizar a transação realizada juntos, ta bom? Simples assim! :)");            
            context.Done(RootDialog.Ok);
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(
                string.Concat(
                    "Desculpe, eu não consegui entender o que você quis dizer.", Environment.NewLine,
                    "Por favor, tenha paciência comigo! Sou um robô bebê! Tenho muito que aprender!"));

            context.Done(RootDialog.Ok);
        }

        [LuisIntent("Expense")]
        public async Task ExpenseAsync(IDialogContext context, LuisResult result)
        {
            await CreateTransactionAsync(context, result, TransactionType.Expense);
        }

        [LuisIntent("Income")]
        public async Task IncomeAsync(IDialogContext context, LuisResult result)
        {
            await CreateTransactionAsync(context, result, TransactionType.Income);
        }

        private async Task CreateTransactionAsync(IDialogContext context, LuisResult result, TransactionType type)
        {
            double? transactionValue = null;

            var moneyEntity = result.Entities.FirstOrDefault(e => string.Equals(e.Type, MoneyEntity));
            if (moneyEntity != null && moneyEntity.Resolution.TryGetValue("value", out object resolutionValue) && double.TryParse(resolutionValue.ToString(), out double value))
            {
                transactionValue = value;
            }
            else
            {
                var numberEntities = result.Entities.Where(e => string.Equals(e.Type, NumberEntity));
                foreach (var numberEntity in numberEntities)
                {
                    if (numberEntity.Resolution.TryGetValue("value", out resolutionValue) && double.TryParse(resolutionValue.ToString(), out value))
                    {
                        transactionValue = value;
                        break;
                    }
                }
            }

            if (transactionValue.HasValue)
            {
                var categories = await FinancialApi.Dispatcher.GetTransactionCategoriesAsync(type);
                categories.Add(OtherCategory);

                PromptDialog.Choice(
                    context,
                    (c, r) => RegisterTransactionAsync(c, r, type, transactionValue.Value, result.Query),
                    categories,
                    "Qual a categoria dessa transação?",
                    "Por favor, selecione uma das opções");
            }
            else
            {
                await context.PostAsync("Você precisa escrever o valor da transação");
                context.Done(RootDialog.Error);
            }
        }

        private async Task RegisterTransactionAsync(IDialogContext context, IAwaitable<TransactionCategory> result, TransactionType type, double value, string description)
        {
            try
            {
                if (await result is TransactionCategory selectedCategory && !string.Equals(selectedCategory.Name, OtherCategory.Name))
                {
                    await FinancialApi.Dispatcher.CreateTransactionAsync(context.GetUserWalletId().Value, type, value, description, selectedCategory.Id);
                    await context.PostAsync("A transação foi registrada com sucesso!");
                    context.Done(RootDialog.Ok);
                }
                else
                {
                    await context.PostAsync("Qual o nome da categoria?");
                    context.Wait((c, r) => RegisterCategoryAndTransactionAsync(c, r, type, value, description));
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Desculpa, não consegui te entender");
                context.Done(RootDialog.Error);
            }
        }

        private async Task RegisterCategoryAndTransactionAsync(IDialogContext context, IAwaitable<object> result, TransactionType type, double value, string description)
        {
            var activity = await result as Activity;
            await FinancialApi.Dispatcher.CreateTransactionAsync(context.GetUserWalletId().Value, type, value, description, activity.Text);
            await context.PostAsync("A transação foi registrada com sucesso!");
            context.Done(RootDialog.Ok);
        }
    }
}