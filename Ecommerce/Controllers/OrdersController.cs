using eCommerce.Data;
using eCommerce.Models;
using eCommerce.Models.ViewModels;
using eCommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public OrdersController(
            IOrderRepository orderRepository,
            IShoppingCartRepository cartRepository,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id);
            if (order == null || order.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            var user = await _userManager.GetUserAsync(User);
            var checkoutViewModel = new CheckoutViewModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Address = user.Address ?? string.Empty,
                City = user.City ?? string.Empty,
                State = user.State ?? string.Empty,
                PostalCode = user.PostalCode ?? string.Empty,
                Country = user.Country ?? string.Empty,
                CartItems = cartItems
            };

            return View(checkoutViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                model.CartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);
                return View("Checkout", model);
            }

            var user = await _userManager.GetUserAsync(User);
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(user.Id);

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Create the order
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity),
                OrderStatus = "Pending",
                ShippingAddress = $"{model.Address}, {model.City}, {model.State} {model.PostalCode}, {model.Country}",
                BillingAddress = $"{model.Address}, {model.City}, {model.State} {model.PostalCode}, {model.Country}",
                PaymentMethod = model.PaymentMethod,
                TransactionId = null  // Set to null or generate a unique ID if needed
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Create order items
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();

            // Clear the cart
            await _cartRepository.ClearCartAsync(user.Id);

            return RedirectToAction(nameof(OrderConfirmation), new { id = order.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id);
            if (order == null || order.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            return View(order);
        }
    }
}

