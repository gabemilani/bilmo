using Financial.Domain.Entities;
using System.Data.Entity;

namespace Financial.Domain.Data
{
    public sealed class BilmoDbContext : DbContext
    {
        public BilmoDbContext()             
            : base("name=BilmoDbConnectionString")
        {

        }

        public DbSet<TransactionCategory> TransactionCategories { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Wallet> Wallets { get; set; }
    }
}