using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Dtos;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private const string CurrentUserId = "default-user";
        private readonly MarketplaceContext _db;

        public CartController(MarketplaceContext db)
        {
            _db = db;
        }

        // GET /api/cart
        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart()
        {
            // .Include and .ThenInclude are critical (Page 10 of lab)
            var cart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == CurrentUserId);

            if (cart is null)
                return NotFound("No cart found for the current user.");

            return Ok(MapToCartResponse(cart));
        }

        // POST /api/cart (The Upsert Pattern - Page 9)
        [HttpPost]
        public async Task<ActionResult<CartItemResponse>> AddToCart([FromBody] AddToCartRequest request)
        {
            var product = await _db.Products.FindAsync(request.ProductId);
            if (product is null)
                return NotFound($"Product {request.ProductId} not found.");

            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == CurrentUserId);

            if (cart is null)
            {
                cart = new Cart { UserId = CurrentUserId };
                _db.Carts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem is not null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                existingItem = new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity  = request.Quantity,
                };
                cart.Items.Add(existingItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Load product info so mapping doesn't fail
            await _db.Entry(existingItem).Reference(i => i.Product).LoadAsync();

            return CreatedAtAction(nameof(GetCart), MapToCartItemResponse(existingItem));
        }

        [HttpPut("{cartItemId}")]
        public async Task<ActionResult<CartItemResponse>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request)
        {
            var cartItem = await _db.CartItems
                .Include(i => i.Cart)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == cartItemId);

            if (cartItem is null) return NotFound();
            if (cartItem.Cart.UserId != CurrentUserId) return Forbid();

            cartItem.Quantity = request.Quantity;
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(MapToCartItemResponse(cartItem));
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var cartItem = await _db.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == cartItemId);

            if (cartItem is null) return NotFound();
            if (cartItem.Cart.UserId != CurrentUserId) return Forbid();

            _db.CartItems.Remove(cartItem);
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == CurrentUserId);

            if (cart is null) return NotFound();

            _db.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Private mapping helpers (Wednesday, Slide 13)
        private static CartResponse MapToCartResponse(Cart cart)
        {
            return new CartResponse
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.Items.Select(MapToCartItemResponse).ToList(),
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
            };
        }

        private static CartItemResponse MapToCartItemResponse(CartItem item) =>
            new CartItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Price = item.Product.Price,
                ImageUrl = item.Product.ImageUrl,
                Quantity = item.Quantity,
            };
    }
}