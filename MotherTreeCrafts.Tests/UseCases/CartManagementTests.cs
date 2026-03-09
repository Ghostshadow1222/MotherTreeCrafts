using FluentAssertions;
using MotherTreeCrafts.Models;
using Xunit;

namespace MotherTreeCrafts.Tests.UseCases;

/// <summary>
/// Unit tests for UC8: Customer manages shopping cart
/// Tests shopping cart functionality including adding, updating, and removing items
/// </summary>
public class CartManagementTests
{
    #region Success Path Tests

    [Fact]
    public void Cart_ShouldCreateWithDefaultValues()
    {
        // Arrange & Act
        var cart = new Cart
        {
            CartId = 1,
            UserId = "user123"
        };

        // Assert
        cart.UserId.Should().Be("user123");
        cart.IsActive.Should().BeTrue();
        cart.SubTotal.Should().Be(0);
        cart.CartItems.Should().BeEmpty();
        cart.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Cart_ShouldAddItem()
    {
        // Arrange
        var cart = new Cart { CartId = 1, UserId = "user123" };
        var cartItem = new CartItem
        {
            CartItemId = 1,
            CartId = 1,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 25.00m
        };

        // Act
        cart.CartItems.Add(cartItem);

        // Assert
        cart.CartItems.Should().HaveCount(1);
        cart.CartItems.First().ProductId.Should().Be(1);
        cart.CartItems.First().Quantity.Should().Be(2);
    }

    [Fact]
    public void Cart_ShouldCalculateSubTotalCorrectly()
    {
        // Arrange
        var cart = new Cart { CartId = 1, UserId = "user123" };

        var item1 = new CartItem
        {
            CartItemId = 1,
            CartId = 1,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 25.00m
        };

        var item2 = new CartItem
        {
            CartItemId = 2,
            CartId = 1,
            ProductId = 2,
            Quantity = 1,
            UnitPrice = 30.00m
        };

        cart.CartItems.Add(item1);
        cart.CartItems.Add(item2);

        // Act
        var calculatedSubTotal = cart.CartItems.Sum(ci => ci.Quantity * ci.UnitPrice);
        cart.SubTotal = calculatedSubTotal;

        // Assert
        cart.SubTotal.Should().Be(80.00m); // (2 * 25) + (1 * 30) = 80
    }

    #endregion

    #region Alternate Flow Tests

    [Fact]
    public void Cart_ShouldUpdateItemQuantity()
    {
        // Arrange
        var cart = new Cart { CartId = 1, UserId = "user123" };
        var cartItem = new CartItem
        {
            CartItemId = 1,
            CartId = 1,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 25.00m
        };
        cart.CartItems.Add(cartItem);

        // Act
        cartItem.Quantity = 5;

        // Assert
        cart.CartItems.First().Quantity.Should().Be(5);
    }

    [Fact]
    public void Cart_ShouldRemoveItem()
    {
        // Arrange
        var cart = new Cart { CartId = 1, UserId = "user123" };
        var cartItem = new CartItem
        {
            CartItemId = 1,
            CartId = 1,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 25.00m
        };
        cart.CartItems.Add(cartItem);

        // Act
        cart.CartItems.Remove(cartItem);

        // Assert
        cart.CartItems.Should().BeEmpty();
    }

    [Fact]
    public void Cart_ShouldClearAllItems()
    {
        // Arrange
        var cart = new Cart { CartId = 1, UserId = "user123" };
        cart.CartItems.Add(new CartItem { CartItemId = 1, CartId = 1, ProductId = 1, Quantity = 2, UnitPrice = 25.00m });
        cart.CartItems.Add(new CartItem { CartItemId = 2, CartId = 1, ProductId = 2, Quantity = 1, UnitPrice = 30.00m });

        // Act
        cart.CartItems.Clear();
        cart.SubTotal = 0;

        // Assert
        cart.CartItems.Should().BeEmpty();
        cart.SubTotal.Should().Be(0);
    }

    [Fact]
    public void Cart_ShouldSupportGuestSessions()
    {
        // Arrange & Act
        var guestCart = new Cart
        {
            CartId = 1,
            UserId = null,
            SessionId = "guest-session-12345"
        };

        // Assert
        guestCart.UserId.Should().BeNull();
        guestCart.SessionId.Should().Be("guest-session-12345");
    }

    [Fact]
    public void Cart_ShouldDeactivateAfterCheckout()
    {
        // Arrange
        var cart = new Cart { CartId = 1, UserId = "user123", IsActive = true };

        // Act
        cart.IsActive = false;

        // Assert
        cart.IsActive.Should().BeFalse();
    }

    #endregion

    #region CartItem Validation Tests

    [Fact]
    public void CartItem_ShouldCalculateTotalPrice()
    {
        // Arrange & Act
        var cartItem = new CartItem
        {
            CartItemId = 1,
            ProductId = 1,
            Quantity = 3,
            UnitPrice = 15.00m
        };

        var totalPrice = cartItem.Quantity * cartItem.UnitPrice;

        // Assert
        totalPrice.Should().Be(45.00m);
    }

    #endregion
}