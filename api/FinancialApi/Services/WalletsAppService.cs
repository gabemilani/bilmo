using AutoMapper;
using Financial.Api.Exceptions;
using Financial.Api.Models;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Financial.Api.Services
{
    public class WalletsAppService
    {
        private readonly TransactionCategoriesService _categoriesService;
        private readonly TransactionsService _transactionsService;
        private readonly WalletsService _walletsService;

        public WalletsAppService(WalletsService walletsService, TransactionsService transactionsService, TransactionCategoriesService categoriesService)
        {
            _walletsService = walletsService ?? throw new ArgumentNullException(nameof(walletsService));
            _transactionsService = transactionsService ?? throw new ArgumentNullException(nameof(transactionsService));
            _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        }

        public async Task<Transaction> AddTransactionAsync(int walletId, TransactionInputModel inputModel)
        {
            if (inputModel.Type == TransactionType.Unknown)
                throw new ArgumentException("Type must be defined");

            if (!inputModel.CategoryId.HasValue && string.IsNullOrWhiteSpace(inputModel.CategoryName))
                throw new ArgumentNullException("Category must be defined");

            var transaction = Mapper.Map<Transaction>(inputModel);
            transaction.Category = await GetCategoryAsync(inputModel.CategoryId, inputModel.CategoryName, inputModel.Type) ?? throw new NotFoundException("Category not found");
            transaction.Wallet = await _walletsService.GetAsync(walletId) ?? throw new NotFoundException("Wallet not found");

            return await _transactionsService.AddAsync(transaction);
        }

        public Task DeleteAsync(int walletId) => _walletsService.DeleteAsync(walletId);

        public Task<Wallet> GetAsync(int walletId) => _walletsService.GetAsync(walletId);

        public TransactionViewModel[] GetTransactions(int walletId, TransactionType? type, int? categoryId, DateTime? fromDate, DateTime? toDate, int? limit)
        {
            var wallet = _walletsService.GetAsync(walletId);
            if (wallet == null)
                throw new NotFoundException("Wallet not found");

            var transactions = _transactionsService.GetMany(walletId, type, categoryId, fromDate, toDate, limit);
            var categoryIds = transactions.Select(t => t.CategoryId).Distinct().ToArray();
            var categories = _categoriesService.GetMany(categoryIds).ToDictionary(c => c.Id);

            return transactions
                .Select(t => new TransactionViewModel
                {
                    CategoryName = categories[t.CategoryId].Name,
                    DateTime = t.DateTime,
                    Description = t.Description,
                    Value = t.Value,
                    Type = t.Type
                })
                .OrderByDescending(t => t.DateTime)
                .ToArray();
        }

        private async Task<TransactionCategory> GetCategoryAsync(int? categoryId, string categoryName, TransactionType type)
        {
            TransactionCategory category = null;

            if (categoryId.HasValue)
                category = await _categoriesService.GetAsync(categoryId.Value);

            if (category == null && !string.IsNullOrEmpty(categoryName))
            {
                category = _categoriesService.GetCategory(categoryName, type);

                if (category == null)
                    category = await _categoriesService.AddAsync(new TransactionCategory { Name = categoryName, Type = type });
            }

            return category;
        }
    }
}