using backend.Dtos;
using backend.Validators;
using Xunit;

namespace backend.Tests.Validators;

public class UpdateCartItemRequestValidatorTests
{
    private readonly UpdateCartItemRequestValidator _validator = new();

    #region Valid Quantity Tests

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    public void Validate_WithValidQuantity_ShouldPass(int quantity)
    {
        // Arrange
        var request = new UpdateCartItemRequest { Quantity = quantity };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion

    #region Invalid Quantity Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    [InlineData(100)]
    [InlineData(101)]
    [InlineData(999)]
    [InlineData(int.MaxValue)]
    public void Validate_WithInvalidQuantity_ShouldFail(int quantity)
    {
        // Arrange
        var request = new UpdateCartItemRequest { Quantity = quantity };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Quantity must be between 1 and 99", result.Errors[0].ErrorMessage);
    }

    #endregion

    #region Boundary Tests

    [Fact]
    public void Validate_WithBoundaryQuantityLower1_ShouldPass()
    {
        // Arrange - Minimum valid quantity
        var request = new UpdateCartItemRequest { Quantity = 1 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithBoundaryQuantityUpper99_ShouldPass()
    {
        // Arrange - Maximum valid quantity
        var request = new UpdateCartItemRequest { Quantity = 99 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithQuantityJustBelowMin_ShouldFail()
    {
        // Arrange
        var request = new UpdateCartItemRequest { Quantity = 0 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithQuantityJustAboveMax_ShouldFail()
    {
        // Arrange
        var request = new UpdateCartItemRequest { Quantity = 100 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    #endregion
}
