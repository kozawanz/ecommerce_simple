using eCommerce.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using eCommerce.Models;
using System.Threading.Tasks;

namespace eCommerce.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartSummaryViewComponent(IShoppingCartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);
            var cartItemCount = userId != null ? await _cartRepository.GetCartItemCountAsync(userId) : 0;
            return View(cartItemCount);
        }
    }
}

