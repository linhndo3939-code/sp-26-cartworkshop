using backend.Dtos;
using Xunit;

namespace backend.Tests.DTOs;

public class CartItemResponseTests
{
    #region LineTotal Calculation - Happy Path

    [Fact]
    public void LineTotal_WithBasicPriceAndQuantity_CalculatesCorrectly()
    {
        // Arrange
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 1
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(10.00m, lineTotal);
    }

    [Fact]
    public void LineTotal_WithMultipleQuantity_CalculatesCorrectly()
    {
        // Arrange
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 49.99m,
            Quantity = 2
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(99.98m, lineTotal);
    }

    #endregion

    #region LineTotal Edge Cases - Boundary Values

    [Fact]
    public void LineTotal_WithMinimumQuantity_CalculatesCorrectly()
    {
        // Arrange - Qty = 1 (minimum)
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 25.00m,
            Quantity = 1
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(25.00m, lineTotal);
    }

    [Fact]
    public void LineTotal_WithMaximumQuantity_CalculatesCorrectly()
    {
        // Arrange - Qty = 99 (maximum allowed)
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 10.00m,
            Quantity = 99
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(990.00m, lineTotal);
    }

    [Fact]
    public void LineTotal_WithSmallestPrice_CalculatesCorrectly()
    {
        // Arrange - Price = $0.01 (smallest cent)
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Penny Item",
            Price = 0.01m,
            Quantity = 99
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(0.99m, lineTotal);
    }

    [Fact]
    public void LineTotal_WithHighestPrice_CalculatesCorrectly()
    {
        // Arrange - Price = $99.99 * Qty = 99
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Premium Item",
            Price = 99.99m,
            Quantity = 99
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(9899.01m, lineTotal);
    }

    #endregion

    #region LineTotal Precision Tests - Decimal Accuracy

    [Fact]
    public void LineTotal_WithFractionalPrice_MaintainsPrecision()
    {
        // Arrange - $0.50 * 3 = $1.50
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 0.50m,
            Quantity = 3
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(1.50m, lineTotal);
    }

    [Fact]
    public void LineTotal_WithThreeCentPrice_MaintainsPrecision()
    {
        // Arrange - $0.33 * 3 = $0.99
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 0.33m,
            Quantity = 3
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(0.99m, lineTotal);
    }

    [Fact]
    public void LineTotal_WithCommonPrice_CalculatesCorrectly()
    {
        // Arrange - $19.99 * 5 = $99.95 (common e-commerce price point)
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 19.99m,
            Quantity = 5
        };

        // Act
        var lineTotal = cartItem.LineTotal;

        // Assert
        Assert.Equal(99.95m, lineTotal);
    }

    #endregion

    #region LineTotal Read-Only Validation

    [Fact]
    public void LineTotal_IsReadOnlyComputedProperty()
    {
        // Arrange
        var cartItem = new CartItemResponse 
        { 
            ProductId = 1,
            ProductName = "Test Product",
            Price = 100.00m,
            Quantity = 2
        };

        // Act & Assert
        // This test documents that LineTotal is computed on-demand
        // and reflects the current Price and Quantity, not cached
        Assert.Equal(200.00m, cartItem.LineTotal);
        
        // If we change the price, LineTotal should reflect it immediately
        cartItem.Price = 150.00m;
        Assert.Equal(300.00m, cartItem.LineTotal);
    }

    #endregion
}
