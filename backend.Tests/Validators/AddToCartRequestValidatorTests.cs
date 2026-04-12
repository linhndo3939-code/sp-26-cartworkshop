using backend.Dtos;
using backend.Validators;
using FluentValidation;
using Xunit;

namespace backend.Tests.Validators;

public class AddToCartRequestValidatorTests
{
    private readonly AddToCartRequestValidator _validator = new();

    #region ProductId Validation

    [Fact]
    public void Validate_WithValidProductId_ShouldPass()
    {
        // Arrange
        var request = new AddToCartRequest { ProductId = 1, Quantity = 5 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithInvalidProductId_ShouldFail(int productId)
    {
        // Arrange
        var request = new AddToCartRequest { ProductId = productId, Quantity = 5 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Product ID must be a positive number", result.Errors[0].ErrorMessage);
    }

    #endregion

    #region Quantity Validation

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    public void Validate_WithValidQuantity_ShouldPass(int quantity)
    {
        // Arrange
        var request = new AddToCartRequest { ProductId = 1, Quantity = quantity };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100)]
    [InlineData(999)]
    public void Validate_WithInvalidQuantity_ShouldFail(int quantity)
    {
        // Arrange
        var request = new AddToCartRequest { ProductId = 1, Quantity = quantity };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("You can only add between 1 and 99 items at a time", result.Errors[0].ErrorMessage);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Validate_WithBothFieldsInvalid_ShouldFailWithMultipleErrors()
    {
        // Arrange
        var request = new AddToCartRequest { ProductId = 0, Quantity = 100 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void Validate_WithBoundaryQuantity1_ShouldPass()
    {
        // Arrange - Minimum valid quantity
        var request = new AddToCartRequest { ProductId = 1, Quantity = 1 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithBoundaryQuantity99_ShouldPass()
    {
        // Arrange - Maximum valid quantity
        var request = new AddToCartRequest { ProductId = 1, Quantity = 99 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion
}
