using eCommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Repositories
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<IEnumerable<ShoppingCart>> GetCartItemsByUserIdAsync(string userId);
        Task<ShoppingCart> GetCartItemAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
    }
}

