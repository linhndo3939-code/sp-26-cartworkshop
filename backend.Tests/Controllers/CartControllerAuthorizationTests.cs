using System.Net;
using System.Net.Http.Json;
using backend.Data;
using backend.Dtos;
using backend.Models;
using backend.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace backend.Tests.Controllers;

public class CartControllerAuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CartControllerAuthorizationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region Happy Path - User Can Update Own Cart Item

    [Fact]
    public async Task UpdateCartItem_WithValidTokenAndOwnItem_ReturnsOkAndUpdatesQuantity()
    {
        // Arrange
        using var client = _factory.CreateClient();
        
        // Setup test data: User A's cart with an item
        await SetupUserACart();
        
        // Generate JWT token for User A (UserId=1)
        var userAToken = JwtTokenGenerator.GenerateToken(userId: 1, username: "userA");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userAToken);

        var updateRequest = new UpdateCartItemRequest { Quantity = 5 };

        // Act
        var response = await client.PutAsJsonAsync("/api/cart/1", updateRequest);

        // Assert - Should succeed
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedItem = await response.Content.ReadFromJsonAsync<CartItemResponse>();
        Assert.NotNull(updatedItem);
        Assert.Equal(5, updatedItem.Quantity);

        // Verify in database that quantity was actually updated
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketplaceContext>();
        var cartItem = await db.CartItems.FirstOrDefaultAsync(ci => ci.Id == 1);
        Assert.NotNull(cartItem);
        Assert.Equal(5, cartItem.Quantity);
    }

    #endregion

    #region Authorization Violation - User Cannot Update Another User's Cart Item (CRITICAL)

    [Fact]
    public async Task UpdateCartItem_WithDifferentUserToken_ReturnsForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Setup: Create User A's cart with a cart item
        await SetupUserACart();

        // Act as User B (UserId=2)
        var userBToken = JwtTokenGenerator.GenerateToken(userId: 2, username: "userB");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userBToken);

        var updateRequest = new UpdateCartItemRequest { Quantity = 10 };

        // Act - User B tries to modify User A's cart item
        var response = await client.PutAsJsonAsync("/api/cart/1", updateRequest);

        // Assert - Should be forbidden (403), NOT 404 or 200
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // Verify in database that quantity was NOT changed (still 1, not 10)
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketplaceContext>();
        var cartItem = await db.CartItems.FirstOrDefaultAsync(ci => ci.Id == 1);
        Assert.NotNull(cartItem);
        Assert.Equal(1, cartItem.Quantity); // Original quantity unchanged
    }

    #endregion

    #region Authorization Verification - No Token Returns Unauthorized

    [Fact]
    public async Task UpdateCartItem_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await SetupUserACart();

        var updateRequest = new UpdateCartItemRequest { Quantity = 5 };

        // Act - No token provided
        var response = await client.PutAsJsonAsync("/api/cart/1", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Edge Case - Expired or Invalid Token

    [Fact]
    public async Task UpdateCartItem_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await SetupUserACart();

        // Use an obviously invalid token
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid.token.here");

        var updateRequest = new UpdateCartItemRequest { Quantity = 5 };

        // Act
        var response = await client.PutAsJsonAsync("/api/cart/1", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Verify Cart Ownership - Other Users Cannot See or Modify

    [Fact]
    public async Task GetCart_WithUserBToken_CannotAccessUserACart()
    {
        // Arrange
        using var client = _factory.CreateClient();
        
        // Setup: User A has a cart
        await SetupUserACart();

        // Act as User B
        var userBToken = JwtTokenGenerator.GenerateToken(userId: 2, username: "userB");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userBToken);

        // Act - User B tries to GET User A's cart
        var response = await client.GetAsync("/api/cart");

        // Assert - User B should get their own empty cart or NotFound (depending on implementation)
        // The key is they should NOT see User A's items
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.OK,
            "User B should either get 404 or empty cart"
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var cartResponse = await response.Content.ReadFromJsonAsync<CartResponse>();
            Assert.Empty(cartResponse.Items); // User B's cart should be empty
        }
    }

    #endregion

    #region Remove Cart Item - Ownership Check

    [Fact]
    public async Task RemoveCartItem_WithDifferentUserToken_ReturnsForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await SetupUserACart();

        // Act as User B
        var userBToken = JwtTokenGenerator.GenerateToken(userId: 2, username: "userB");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userBToken);

        // Act - User B tries to remove User A's cart item
        var response = await client.DeleteAsync("/api/cart/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // Verify item still exists in database
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketplaceContext>();
        var cartItem = await db.CartItems.FirstOrDefaultAsync(ci => ci.Id == 1);
        Assert.NotNull(cartItem);
    }

    #endregion

    #region Helper Methods

    private async Task SetupUserACart()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketplaceContext>();

        // Ensure database has test data
        db.Database.EnsureCreated();

        // Check if User A already has a cart
        var existingCart = await db.Carts.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == "1");
        if (existingCart != null)
        {
            return; // Cart already exists
        }

        // Create User A's cart
        var cart = new Cart { UserId = "1" };
        db.Carts.Add(cart);
        await db.SaveChangesAsync();

        // Create a cart item for User A's cart
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == 1);
        if (product == null)
        {
            throw new InvalidOperationException("Product with ID 1 not found. Ensure test data is seeded.");
        }

        var cartItem = new CartItem 
        { 
            CartId = cart.Id,
            ProductId = product.Id,
            Quantity = 1
        };
        db.CartItems.Add(cartItem);
        await db.SaveChangesAsync();
    }

    #endregion
}
