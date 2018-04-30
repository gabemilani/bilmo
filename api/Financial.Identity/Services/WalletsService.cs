using Financial.Domain.Data;
using Financial.Domain.Entities;
using Financial.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Financial.Domain.Services
{
    public class WalletsService : IService
    {
        private readonly BilmoDbContext _context;

        public WalletsService(BilmoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Wallet> AddAsync(Wallet wallet)
        {
            if (string.IsNullOrWhiteSpace(wallet.Name))
                throw new ArgumentNullException(nameof(wallet.Name), "Name must be defined");
                        
            _context.Users.Attach(wallet.User);
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task DeleteAsync(int walletId)
        {
            var wallet = await GetAsync(walletId);
            if (wallet == null)
                return;

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }

        public Task<Wallet> GetAsync(int id) => _context.Wallets.FindAsync(id);

        public Wallet[] GetUserWalletsAsync(int userId) => _context.Wallets.Where(w => w.UserId == userId).ToArray();
    }
}