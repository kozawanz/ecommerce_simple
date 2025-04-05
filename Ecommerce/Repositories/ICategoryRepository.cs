using eCommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerce.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
    }
}

