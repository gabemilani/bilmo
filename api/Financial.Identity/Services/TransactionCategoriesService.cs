using Financial.Domain.Data;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financial.Domain.Services
{
    public class TransactionCategoriesService : IService
    {
        private readonly BilmoDbContext _context;

        public TransactionCategoriesService(BilmoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TransactionCategory> AddAsync(TransactionCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentNullException(nameof(category.Name), "Name must be defined");

            if (category.Type == TransactionType.Unknown)
                throw new ArgumentNullException(nameof(category.Type), "Typ must be defined");

            if (GetCategory(category.Name, category.Type) != null)
                throw new ArgumentException("This category already exists");

            _context.TransactionCategories.Add(category);

            await _context.SaveChangesAsync();

            return category;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.TransactionCategories.FindAsync(id);
            if (category == null)
                return;

            _context.TransactionCategories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public TransactionCategory[] GetMany(TransactionType type)
        {
            var result = _context.TransactionCategories as IEnumerable<TransactionCategory>;

            if (type != TransactionType.Unknown)
                result = result.Where(c => c.Type == type);

            return result.ToArray();
        }

        public TransactionCategory[] GetMany(int[] ids)
        {
            if (ids?.Any() != true)
                return new TransactionCategory[0];

            return _context.TransactionCategories.Where(c => ids.Contains(c.Id)).ToArray();
        }

        public Task<TransactionCategory> GetAsync(int id) => _context.TransactionCategories.FindAsync(id);

        public TransactionCategory GetCategory(string name, TransactionType type)
        {
            if (string.IsNullOrEmpty(name) || type == TransactionType.Unknown)
                return null;

            return _context.TransactionCategories.SingleOrDefault(c => c.Name == name && type == c.Type);
        }
    }
}