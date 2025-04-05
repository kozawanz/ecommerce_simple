using eCommerce.Data;
using eCommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ShoppingCart>> GetCartItemsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<ShoppingCart> GetCartItemAsync(string userId, int productId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _dbSet.Where(c => c.UserId == userId).ToListAsync();
            _dbSet.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }
    }
}

