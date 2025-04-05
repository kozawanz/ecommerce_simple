using eCommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task AddAsync(Product entity);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task SaveChangesAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    }
}

