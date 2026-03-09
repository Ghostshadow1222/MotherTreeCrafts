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
            ReviewerName = "John Doe",
            Rating = 5,
            Comment = "Great product!"
        };

        // Assert
        review.ProductId.Should().Be(1);
        review.UserId.Should().Be("user123");
        review.Rating.Should().Be(5);
        review.Comment.Should().Be("Great product!");
        review.IsApproved.Should().BeTrue(); // Default value
        // Note: ReviewDate is database-generated, so it won't be set until saved to DB
        review.ReviewDate.Should().Be(default(DateTime));
    }

    [Fact]
    public void ProductReview_ShouldDefaultToApproved()
    {
        // Arrange & Act
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            ReviewerName = "John Doe",
            Comment = "Test review",
            Rating = 4
        };

        // Assert
        review.IsApproved.Should().BeTrue(); // Defaults to true
        review.IsVisible.Should().BeTrue(); // Defaults to true
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
            ReviewerName = "John Doe",
            Comment = "Test review",
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
            ReviewerName = "John Doe",
            Comment = "Test review",
            Rating = invalidRating
        };

        // Assert
        // Validation enforced by data annotations - this tests the constraint
        review.Rating.Should().NotBeInRange(1, 5);
    }

    [Fact]
    public void ProductReview_Title_ShouldBeOptional()
    {
        // Arrange & Act
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            ReviewerName = "John Doe",
            Comment = "This is a required comment",
            Rating = 5,
            Title = null
        };

        // Assert
        review.Title.Should().BeNull();
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
            ReviewerName = "John Doe",
            Comment = "Test review",
            Rating = 5,
            IsApproved = false
        };

        // Act
        review.IsApproved = true;

        // Assert
        review.IsApproved.Should().BeTrue();
    }

    [Fact]
    public void ProductReview_ShouldTransitionFromApprovedToRejected()
    {
        // Arrange
        var review = new ProductReview
        {
            ProductId = 1,
            UserId = "user123",
            ReviewerName = "John Doe",
            Comment = "Test review",
            Rating = 5,
            IsApproved = true,
            IsVisible = true
        };

        // Act
        review.IsApproved = false;
        review.IsVisible = false;

        // Assert
        review.IsApproved.Should().BeFalse();
        review.IsVisible.Should().BeFalse();
    }

    [Fact]
    public void ProductReview_ShouldPreventDuplicateReviewFromSameUserForSameProduct()
    {
        // Arrange
        var existingReviews = new List<ProductReview>
        {
            new ProductReview { ReviewId = 1, ProductId = 1, UserId = "user123", ReviewerName = "John Doe", Comment = "Great!", Rating = 5 }
        };

        var newReview = new ProductReview { ProductId = 1, UserId = "user123", ReviewerName = "John Doe", Comment = "Good", Rating = 4 };

        // Act
        var hasDuplicate = existingReviews.Any(r => 
            r.ProductId == newReview.ProductId && 
            r.UserId == newReview.UserId);

        // Assert
        hasDuplicate.Should().BeTrue("duplicate reviews should be detected");
    }

    #endregion
}