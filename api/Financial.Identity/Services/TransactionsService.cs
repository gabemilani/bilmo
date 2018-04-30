using Financial.Domain.Data;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Financial.Domain.Services
{
    public class TransactionsService : IService
    {
        private readonly BilmoDbContext _context;

        public TransactionsService(BilmoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Unknown)
                throw new ArgumentException(nameof(transaction.Type), "Type must be defined");

            transaction.DateTime = DateTime.UtcNow.AddHours(-3);

            _context.TransactionCategories.Attach(transaction.Category);
            _context.Wallets.Attach(transaction.Wallet);

            transaction.Wallet.CurrentBalance += transaction.Type == TransactionType.Expense ? -transaction.Value : transaction.Value;

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return transaction;
        }

        public Transaction[] GetMany(int walletId, TransactionType? type, int? categoryId, DateTime? fromDate, DateTime? toDate, int? limit)
        {
            var transactions = _context.Transactions.AsQueryable();
            
            transactions = transactions.Where(t => t.WalletId == walletId).OrderByDescending(t => t.DateTime);

            if (type.HasValue && type.Value != TransactionType.Unknown)
                transactions = transactions.Where(t => t.Type == type.Value);

            if (categoryId.HasValue && categoryId.Value > 0)
                transactions = transactions.Where(t => t.CategoryId == categoryId.Value);

            if (fromDate.HasValue && fromDate.Value > default(DateTime))
                transactions = transactions.Where(t => t.DateTime > fromDate.Value);

            if (toDate.HasValue && toDate.Value > default(DateTime))
                transactions = transactions.Where(t => t.DateTime < toDate.Value);

            if (limit.HasValue)
                transactions = transactions.Take(limit.Value);

            return transactions.ToArray();
        }
    }
}