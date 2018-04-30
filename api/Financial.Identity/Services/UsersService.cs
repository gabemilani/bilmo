using Financial.Domain.Data;
using Financial.Domain.Entities;
using Financial.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Financial.Domain.Services
{
    public class UsersService : IService
    {
        private readonly BilmoDbContext _context;

        public UsersService(BilmoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> AddAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentNullException(nameof(user.Name), "Name must be defined");

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }        

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return;

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }

        public Task<User> GetAsync(int id) => _context.Users.FindAsync(id);

        public User[] GetAll() => _context.Users.ToArray();

        public async Task<User> ReplaceAsync(int id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                _context.Users.Remove(existingUser);

            user.Id = id;

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}