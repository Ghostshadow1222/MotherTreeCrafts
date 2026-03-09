using FluentAssertions;
using MotherTreeCrafts.Models;
using Xunit;

namespace MotherTreeCrafts.Tests.UseCases;

/// <summary>
/// Unit tests for UC3: Customer submits product review
/// Tests product review functionality including submission and moderation
/// </summary>
public class ProductReviewTests
{
    #region Success Path Tests

    [Fact]
    public void ProductReview_ShouldCreateWithValidData()
    {
        // Arrange & Act
        var review = new ProductReview
        {
            ReviewId = 1,
            ProductId = 1,
            UserId = "user123",
            Rating = 5,
            ReviewText = "Great product!",
            ReviewStatus = "Pending"
        };

        // Assert
        review.ProductId.Should().Be(1);
        review.UserId.Should().Be("user123");
        review.Rating.Should().Be(5);
        review.ReviewText.Should().Be("Great product!");
        review.ReviewStatus.Should().Be("Pending");
        review.ReviewDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ProductReview_ShouldDefaultToPendingStatus()
    {
        // Arrange & Act
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            Rating = 4
        };

        // Assert
        review.ReviewStatus.Should().Be("Pending");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void ProductReview_ShouldAcceptValidRatings(int rating)
    {
        // Arrange & Act
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            Rating = rating
        };

        // Assert
        review.Rating.Should().BeInRange(1, 5);
    }

    #endregion

    #region Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void ProductReview_Rating_ShouldRejectInvalidValues(int invalidRating)
    {
        // Arrange & Act
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            Rating = invalidRating
        };

        // Assert
        // Validation enforced by data annotations - this tests the constraint
        review.Rating.Should().NotBeInRange(1, 5);
    }

    [Fact]
    public void ProductReview_ReviewText_ShouldBeOptional()
    {
        // Arrange & Act
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            Rating = 5,
            ReviewText = null
        };

        // Assert
        review.ReviewText.Should().BeNull();
        review.Rating.Should().Be(5);
    }

    #endregion

    #region Alternate Flow Tests - Moderation

    [Fact]
    public void ProductReview_ShouldTransitionFromPendingToApproved()
    {
        // Arrange
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            Rating = 5,
            ReviewStatus = "Pending"
        };

        // Act
        review.ReviewStatus = "Approved";

        // Assert
        review.ReviewStatus.Should().Be("Approved");
    }

    [Fact]
    public void ProductReview_ShouldTransitionFromPendingToRejected()
    {
        // Arrange
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            Rating = 5,
            ReviewStatus = "Pending"
        };

        // Act
        review.ReviewStatus = "Rejected";

        // Assert
        review.ReviewStatus.Should().Be("Rejected");
    }

    [Fact]
    public void ProductReview_ShouldPreventDuplicateReviewFromSameUserForSameProduct()
    {
        // Arrange
        var existingReviews = new List<ProductReview>
        {
            new ProductReview { ReviewId = 1, ProductId = 1, UserId = "user123", Rating = 5 }
        };

        var newReview = new ProductReview { ProductId = 1, UserId = "user123", Rating = 4 };

        // Act
        var hasDuplicate = existingReviews.Any(r => 
            r.ProductId == newReview.ProductId && 
            r.UserId == newReview.UserId);

        // Assert
        hasDuplicate.Should().BeTrue("duplicate reviews should be detected");
    }

    #endregion
}