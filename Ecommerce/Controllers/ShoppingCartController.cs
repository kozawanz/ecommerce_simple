using eCommerce.Models;
using eCommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(
            IShoppingCartRepository cartRepository,
            IProductRepository productRepository,
            UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            var cartItem = await _cartRepository.GetCartItemAsync(userId, productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                await _cartRepository.UpdateAsync(cartItem);
            }
            else
            {
                cartItem = new ShoppingCart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _cartRepository.AddAsync(cartItem);
            }

            await _cartRepository.SaveChangesAsync();

            // Return a JSON response instead of redirecting
            return Json(new { success = true, message = "Product added to cart" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = await _cartRepository.GetCartItemAsync(userId, productId);

            if (cartItem == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                await _cartRepository.RemoveAsync(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                await _cartRepository.UpdateAsync(cartItem);
            }

            await _cartRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = await _cartRepository.GetCartItemAsync(userId, productId);

            if (cartItem == null)
            {
                return NotFound();
            }

            await _cartRepository.RemoveAsync(cartItem);
            await _cartRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = _userManager.GetUserId(User);
            await _cartRepository.ClearCartAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            var count = await _cartRepository.GetCartItemCountAsync(userId);
            return Json(new { count });
        }
    }
}

