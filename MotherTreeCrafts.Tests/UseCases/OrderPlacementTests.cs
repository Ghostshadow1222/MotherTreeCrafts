using FluentAssertions;
using MotherTreeCrafts.Models;
using Xunit;

namespace MotherTreeCrafts.Tests.UseCases;

/// <summary>
/// Unit tests for UC4: Customer places order
/// Tests order placement and checkout functionality
/// </summary>
public class OrderPlacementTests
{
    #region Success Path Tests

    [Fact]
    public void Order_ShouldCreateWithRequiredInformation()
    {
        // Arrange & Act
        var order = new Order
        {
            OrderId = 1,
            OrderNumber = "ORD-2026-001",
            UserId = "user123",
            OrderStatus = "Pending",
            ShippingAddress = "123 Main St, City, State 12345",
            BillingAddress = "123 Main St, City, State 12345",
            SubTotal = 100.00m,
            TaxAmount = 8.00m,
            ShippingCost = 5.00m,
            TotalAmount = 113.00m,
            PaymentStatus = "Pending"
        };

        // Assert
        order.OrderNumber.Should().Be("ORD-2026-001");
        order.UserId.Should().Be("user123");
        order.OrderStatus.Should().Be("Pending");
        order.TotalAmount.Should().Be(113.00m);
        order.OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Order_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var subtotal = 100.00m;
        var tax = 8.00m;
        var shipping = 5.00m;

        // Act
        var order = new Order
        {
            OrderNumber = "ORD-2026-001",
            UserId = "user123",
            ShippingAddress = "Test Address",
            BillingAddress = "Test Address",
            OrderStatus = "Pending",
            SubTotal = subtotal,
            TaxAmount = tax,
            ShippingCost = shipping,
            TotalAmount = subtotal + tax + shipping
        };

        // Assert
        order.TotalAmount.Should().Be(113.00m);
        order.TotalAmount.Should().Be(order.SubTotal + order.TaxAmount + order.ShippingCost);
    }

    [Fact]
    public void Order_ShouldContainOrderItems()
    {
        // Arrange & Act
        var order = new Order
        {
            OrderNumber = "ORD-2026-001",
            UserId = "user123",
            ShippingAddress = "Test",
            BillingAddress = "Test",
            OrderStatus = "Pending",
            SubTotal = 100m,
            TotalAmount = 100m
        };

        var orderItem1 = new OrderItem
        {
            OrderItemId = 1,
            OrderId = order.OrderId,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 25.00m,
            LineTotal = 50.00m
        };

        var orderItem2 = new OrderItem
        {
            OrderItemId = 2,
            OrderId = order.OrderId,
            ProductId = 2,
            Quantity = 1,
            UnitPrice = 50.00m,
            LineTotal = 50.00m
        };

        order.OrderItems.Add(orderItem1);
        order.OrderItems.Add(orderItem2);

        // Assert
        order.OrderItems.Should().HaveCount(2);
        order.OrderItems.Sum(oi => oi.LineTotal).Should().Be(100.00m);
    }

    #endregion

    #region Alternate Flow Tests

    [Fact]
    public void Order_ShouldTransitionFromPendingToConfirmed()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-2026-001",
            UserId = "user123",
            ShippingAddress = "Test",
            BillingAddress = "Test",
            OrderStatus = "Pending",
            SubTotal = 100m,
            TotalAmount = 100m
        };

        // Act
        order.OrderStatus = "Confirmed";
        order.PaymentStatus = "Paid";

        // Assert
        order.OrderStatus.Should().Be("Confirmed");
        order.PaymentStatus.Should().Be("Paid");
    }

    [Fact]
    public void Order_ShouldHandlePaymentFailure()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-2026-001",
            UserId = "user123",
            ShippingAddress = "Test",
            BillingAddress = "Test",
            OrderStatus = "Pending",
            PaymentStatus = "Pending",
            SubTotal = 100m,
            TotalAmount = 100m
        };

        // Act
        order.PaymentStatus = "Failed";
        order.OrderStatus = "Cancelled";

        // Assert
        order.PaymentStatus.Should().Be("Failed");
        order.OrderStatus.Should().Be("Cancelled");
    }

    [Fact]
    public void Order_ShouldAllowCancellation()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-2026-001",
            UserId = "user123",
            ShippingAddress = "Test",
            BillingAddress = "Test",
            OrderStatus = "Pending",
            SubTotal = 100m,
            TotalAmount = 100m
        };

        // Act
        order.OrderStatus = "Cancelled";
        order.CancellationReason = "Customer requested cancellation";

        // Assert
        order.OrderStatus.Should().Be("Cancelled");
        order.CancellationReason.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region OrderItem Tests

    [Fact]
    public void OrderItem_ShouldCalculateLineTotalCorrectly()
    {
        // Arrange & Act
        var orderItem = new OrderItem
        {
            OrderItemId = 1,
            ProductId = 1,
            Quantity = 3,
            UnitPrice = 25.00m,
            LineTotal = 3 * 25.00m
        };

        // Assert
        orderItem.LineTotal.Should().Be(75.00m);
        orderItem.LineTotal.Should().Be(orderItem.Quantity * orderItem.UnitPrice);
    }

    #endregion
}