using Financial.Bot.Extensions;
using Financial.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Financial.Bot.Utils
{
    public class FinancialApi
    {
        private readonly HttpClient _httpClient;

        private FinancialApi()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(AppSettings.FinancialApiUrl),
                Timeout = new TimeSpan(0, 1, 0)
            };

            var scheme = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{AppSettings.FinancialApiAuthUsername}:{AppSettings.FinancialApiAuthPassword}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", scheme);
        }

        public static FinancialApi Dispatcher { get; } = new FinancialApi();

        public async Task CreateTransactionAsync(int walletId, TransactionType type, double value, string description, string categoryName)
        {
            var transaction = new Transaction
            {
                CategoryName = categoryName,
                Description = description,
                Type = type,
                Value = value
            };

            await _httpClient.PostObjectAsJsonAsync($"wallets/{walletId}/transactions", transaction);
        }

        public async Task CreateTransactionAsync(int walletId, TransactionType type, double value, string description, int categoryId)
        {
            var transaction = new Transaction
            {
                CategoryId = categoryId,
                Description = description,
                Type = type,
                Value = value
            };

            await _httpClient.PostObjectAsJsonAsync($"wallets/{walletId}/transactions", transaction);
        }

        public async Task<int> CreateUserAsync(string name, string email = null)
        {
            var user = new User { Name = name, Email = email };
            var location = await _httpClient.PostObjectAsJsonAsync("users", user);
            var id = location.AbsolutePath.Split('/').Last();
            return int.Parse(id);
        }

        public async Task<int> CreateWalletAsync(int userId, double currentBalance)
        {
            var wallet = new Wallet { Name = "Minha Carteira", CurrentBalance = currentBalance };
            var location = await _httpClient.PostObjectAsJsonAsync($"users/{userId}/wallets", wallet);
            var id = location.AbsolutePath.Split('/').Last();
            return int.Parse(id);
        }

        public Task<Transaction[]> GetTransactionsAsync(int walletId, int limit) 
        {
            var relativeUrl = $"wallets/{walletId}/transactions?limit={limit}";
            return _httpClient.GetJsonObjectAsync<Transaction[]>(relativeUrl);
        }

        public Task<Transaction[]> GetTransactionsAsync(int walletId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var relativeUrl = $"wallets/{walletId}/transactions";
            
            if (fromDate.HasValue && toDate.HasValue)
                relativeUrl += $"?fromDate={fromDate}&toDate={toDate}";
            else if (fromDate.HasValue)
                relativeUrl += $"?fromDate={fromDate}";
            else if (toDate.HasValue)
                relativeUrl += $"?toDate={toDate}";

            return _httpClient.GetJsonObjectAsync<Transaction[]>(relativeUrl);
        }

        public Task<List<TransactionCategory>> GetTransactionCategoriesAsync(TransactionType type)
        {
            var relativeUrl = "transaction-categories";
            if (type != TransactionType.Unknown)
                relativeUrl += $"?type={type}";

            return _httpClient.GetJsonObjectAsync<List<TransactionCategory>>(relativeUrl);
        }

        public Task<User> GetUserAsync(int id)
        {
            return _httpClient.GetJsonObjectAsync<User>($"users/{id}");
        }

        public Task<Wallet[]> GetUserWalletsAsync(int userId)
        {
            return _httpClient.GetJsonObjectAsync<Wallet[]>($"users/{userId}/wallets");
        }

        public Task<Wallet> GetWalletAsync(int walletId)
        {
            return _httpClient.GetJsonObjectAsync<Wallet>($"wallets/{walletId}");
        }

        public Task<Transaction[]> GetWalletTransactionsAsync(int walletId)
        {
            return _httpClient.GetJsonObjectAsync<Transaction[]>($"wallets/{walletId}/transactions");
        }
    }
}