using eCommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order> GetOrderWithItemsAsync(int orderId);
    }
}

