using eCommerce.Models;
using eCommerce.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eCommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(int? categoryId, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchResults = await _productRepository.SearchProductsAsync(searchTerm);
                return View(searchResults);
            }
            else if (categoryId.HasValue)
            {
                var productsByCategory = await _productRepository.GetProductsByCategoryAsync(categoryId.Value);
                return View(productsByCategory);
            }
            else
            {
                var allProducts = await _productRepository.GetAllAsync();
                return View(allProducts);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryRepository.GetCategoriesWithProductsAsync();
            return View(categories);
        }
    }
}

