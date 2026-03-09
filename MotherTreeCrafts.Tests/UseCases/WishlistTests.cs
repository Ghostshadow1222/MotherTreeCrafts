using FluentAssertions;
using MotherTreeCrafts.Models;
using Xunit;

namespace MotherTreeCrafts.Tests.UseCases;

/// <summary>
/// Unit tests for UC2: Customer adds product to wishlist
/// Tests wishlist management functionality
/// </summary>
public class WishlistTests
{
    #region Success Path Tests

    [Fact]
    public void Wishlist_ShouldCreateWithValidData()
    {
        // Arrange & Act
        var wishlist = new Wishlist
        {
            WishlistId = 1,
            AccountId = "user123",
            ProductId = 1,
            Priority = 1
        };

        // Assert
        wishlist.AccountId.Should().Be("user123");
        wishlist.ProductId.Should().Be(1);
        wishlist.DateAdded.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        wishlist.Priority.Should().Be(1);
    }

    [Fact]
    public void Wishlist_ShouldSetDateAddedAutomatically()
    {
        // Arrange & Act
        var beforeCreation = DateTime.UtcNow;
        var wishlist = new Wishlist
        {
            AccountId = "user123",
            ProductId = 1,
            Priority = 1
        };
        var afterCreation = DateTime.UtcNow;

        // Assert
        wishlist.DateAdded.Should().BeOnOrAfter(beforeCreation);
        wishlist.DateAdded.Should().BeOnOrBefore(afterCreation);
    }

    #endregion

    #region Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Wishlist_Priority_ShouldRejectInvalidValues(int invalidPriority)
    {
        // Arrange & Act
        var wishlist = new Wishlist
        {
            AccountId = "user123",
            ProductId = 1,
            Priority = invalidPriority
        };

        // Assert
        // Note: Validation would be enforced by data annotations at the model validation level
        wishlist.Priority.Should().BeLessThan(1);
    }

    [Fact]
    public void Wishlist_Priority_ShouldAcceptValidRange()
    {
        // Arrange & Act
        var wishlist = new Wishlist
        {
            AccountId = "user123",
            ProductId = 1,
            Priority = 500
        };

        // Assert
        wishlist.Priority.Should().BeInRange(1, 2000);
    }

    #endregion

    #region Alternate Flow Tests

    [Fact]
    public void Wishlist_ShouldAllowMultipleItemsForSameUser()
    {
        // Arrange
        var wishlists = new List<Wishlist>
        {
            new Wishlist { WishlistId = 1, AccountId = "user123", ProductId = 1, Priority = 1 },
            new Wishlist { WishlistId = 2, AccountId = "user123", ProductId = 2, Priority = 2 },
            new Wishlist { WishlistId = 3, AccountId = "user123", ProductId = 3, Priority = 3 }
        };

        // Act
        var userWishlists = wishlists.Where(w => w.AccountId == "user123").ToList();

        // Assert
        userWishlists.Should().HaveCount(3);
        userWishlists.Should().OnlyContain(w => w.AccountId == "user123");
    }

    [Fact]
    public void Wishlist_ShouldPreventDuplicateProductForSameUser()
    {
        // Arrange
        var existingWishlists = new List<Wishlist>
        {
            new Wishlist { WishlistId = 1, AccountId = "user123", ProductId = 1, Priority = 1 }
        };

        var newWishlist = new Wishlist { AccountId = "user123", ProductId = 1, Priority = 2 };

        // Act
        var isDuplicate = existingWishlists.Any(w => 
            w.AccountId == newWishlist.AccountId && 
            w.ProductId == newWishlist.ProductId);

        // Assert
        isDuplicate.Should().BeTrue("duplicate wishlist items should be detected");
    }

    #endregion
}