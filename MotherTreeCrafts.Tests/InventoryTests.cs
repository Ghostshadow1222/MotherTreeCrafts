using FluentAssertions;
using MotherTreeCrafts.Models;
using Xunit;

namespace MotherTreeCrafts.Tests;

/// <summary>
/// Unit tests for the Inventory class
/// </summary>
public class InventoryTests
{
    #region Constructor and Default Values Tests

    [Fact]
    public void Inventory_ShouldHaveCorrectDefaultValues()
    {
      // Arrange & Act
        var inventory = new Inventory();

        // Assert
        inventory.ReorderLevel.Should().Be(Inventory.DefaultReorderLevel);
        inventory.MaxStockLevel.Should().Be(Inventory.DefaultMaxStockLevel);
        inventory.ReservedQuantity.Should().Be(Inventory.DefaultReservedQuantity);
        inventory.QuantityOnHand.Should().Be(0);
        inventory.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region Property Validation Tests

    [Fact]
    public void QuantityOnHand_ShouldThrowException_WhenSetToNegative()
    {
   // Arrange
        var inventory = new Inventory();

        // Act
      Action act = () => inventory.QuantityOnHand = -1;

        // Assert
        act.Should().Throw<ArgumentException>()
  .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void QuantityOnHand_ShouldUpdateLastModified_WhenChanged()
    {
      // Arrange
        var inventory = new Inventory { QuantityOnHand = 10 };
    var originalTime = inventory.LastUpdated;
        Thread.Sleep(10); // Small delay to ensure time difference

        // Act
    inventory.QuantityOnHand = 20;

        // Assert
        inventory.LastUpdated.Should().BeAfter(originalTime);
}

    [Fact]
    public void ReorderLevel_ShouldThrowException_WhenSetToNegative()
    {
        // Arrange
        var inventory = new Inventory();

   // Act
        Action act = () => inventory.ReorderLevel = -1;

 // Assert
        act.Should().Throw<ArgumentException>()
       .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void MaxStockLevel_ShouldThrowException_WhenSetToNegative()
    {
        // Arrange
        var inventory = new Inventory();

        // Act
  Action act = () => inventory.MaxStockLevel = -1;

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void MaxStockLevel_ShouldThrowException_WhenLessThanReorderLevel()
    {
        // Arrange
        var inventory = new Inventory { ReorderLevel = 10 };

        // Act
        Action act = () => inventory.MaxStockLevel = 5;

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be less than reorder level*");
    }

    #endregion

    #region Computed Properties Tests

    [Theory]
    [InlineData(100, 20, 80)]
    [InlineData(50, 10, 40)]  // Changed from (50, 0, 50) to (50, 10, 40)
    [InlineData(25, 25, 0)]
    public void AvailableQuantity_ShouldReturnCorrectValue(int onHand, int reserved, int expected)
    {
        // Arrange
        var inventory = new Inventory
        {
  QuantityOnHand = onHand
 };
     inventory.ReserveStock(reserved);

      // Act & Assert
        inventory.AvailableQuantity.Should().Be(expected);
    }

    [Theory]
    [InlineData(10, true)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void IsInStock_ShouldReturnCorrectValue_BasedOnAvailableQuantity(int available, bool expected)
    {
        // Arrange
        var inventory = new Inventory
        {
       QuantityOnHand = available
        };

        // Act & Assert
        inventory.IsInStock.Should().Be(expected);
    }

    [Theory]
    [InlineData(10, 5, false)]  // Above reorder level
    [InlineData(5, 5, true)]    // At reorder level
    [InlineData(3, 5, true)]    // Below reorder level
    public void NeedsReorder_ShouldReturnCorrectValue(int onHand, int reorderLevel, bool expected)
    {
        // Arrange
        var inventory = new Inventory
        {
     QuantityOnHand = onHand,
     ReorderLevel = reorderLevel
        };

        // Act & Assert
 inventory.NeedsReorder.Should().Be(expected);
    }

    #endregion

    #region ReserveStock Tests

    [Fact]
    public void ReserveStock_ShouldIncreaseReservedQuantity_WhenSufficientStock()
    {
        // Arrange
  var inventory = new Inventory { QuantityOnHand = 100 };

 // Act
        inventory.ReserveStock(30);

     // Assert
        inventory.ReservedQuantity.Should().Be(30);
     inventory.AvailableQuantity.Should().Be(70);
    }

    [Fact]
    public void ReserveStock_ShouldThrowException_WhenQuantityIsNegative()
    {
   // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

      // Act
      Action act = () => inventory.ReserveStock(-10);

  // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*must be positive*");
    }

    [Fact]
    public void ReserveStock_ShouldThrowException_WhenQuantityIsZero()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

  // Act
   Action act = () => inventory.ReserveStock(0);

        // Assert
    act.Should().Throw<ArgumentException>()
        .WithMessage("*must be positive*");
    }

    [Fact]
    public void ReserveStock_ShouldThrowException_WhenInsufficientStock()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 50 };

    // Act
        Action act = () => inventory.ReserveStock(60);

        // Assert
        act.Should().Throw<InvalidOperationException>()
          .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public void ReserveStock_ShouldAllowMultipleReservations()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

        // Act
        inventory.ReserveStock(20);
        inventory.ReserveStock(30);

        // Assert
        inventory.ReservedQuantity.Should().Be(50);
        inventory.AvailableQuantity.Should().Be(50);
    }

    #endregion

  #region ReleaseStock Tests

    [Fact]
    public void ReleaseStock_ShouldDecreaseReservedQuantity()
    {
  // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };
        inventory.ReserveStock(50);

        // Act
        inventory.ReleaseStock(20);

    // Assert
        inventory.ReservedQuantity.Should().Be(30);
        inventory.AvailableQuantity.Should().Be(70);
    }

    [Fact]
    public void ReleaseStock_ShouldThrowException_WhenQuantityIsNegative()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };
        inventory.ReserveStock(50);

        // Act
        Action act = () => inventory.ReleaseStock(-10);

        // Assert
     act.Should().Throw<ArgumentException>()
            .WithMessage("*must be positive*");
    }

    [Fact]
    public void ReleaseStock_ShouldThrowException_WhenQuantityIsZero()
  {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };
        inventory.ReserveStock(50);

     // Act
        Action act = () => inventory.ReleaseStock(0);

        // Assert
        act.Should().Throw<ArgumentException>()
        .WithMessage("*must be positive*");
    }

    [Fact]
    public void ReleaseStock_ShouldThrowException_WhenReleasingMoreThanReserved()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };
     inventory.ReserveStock(30);

        // Act
        Action act = () => inventory.ReleaseStock(50);

        // Assert
        act.Should().Throw<InvalidOperationException>()
        .WithMessage("*Cannot release more than reserved*");
    }

    #endregion

    #region AddStock Tests

    [Fact]
    public void AddStock_ShouldIncreaseQuantityOnHand()
  {
 // Arrange
        var inventory = new Inventory { QuantityOnHand = 50 };

   // Act
        inventory.AddStock(25);

      // Assert
        inventory.QuantityOnHand.Should().Be(75);
    }

    [Fact]
    public void AddStock_ShouldThrowException_WhenQuantityIsNegative()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 50 };

        // Act
        Action act = () => inventory.AddStock(-10);

        // Assert
        act.Should().Throw<ArgumentException>()
     .WithMessage("*must be positive*");
    }

    [Fact]
    public void AddStock_ShouldThrowException_WhenQuantityIsZero()
    {
        // Arrange
  var inventory = new Inventory { QuantityOnHand = 50 };

        // Act
        Action act = () => inventory.AddStock(0);

        // Assert
     act.Should().Throw<ArgumentException>()
   .WithMessage("*must be positive*");
    }

    [Fact]
    public void AddStock_ShouldUpdateLastModified()
    {
    // Arrange
        var inventory = new Inventory { QuantityOnHand = 50 };
    var originalTime = inventory.LastUpdated;
  Thread.Sleep(10);

        // Act
        inventory.AddStock(25);

        // Assert
        inventory.LastUpdated.Should().BeAfter(originalTime);
    }

    #endregion

    #region RemoveStock Tests

  [Fact]
    public void RemoveStock_ShouldDecreaseQuantityOnHand()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

        // Act
        inventory.RemoveStock(30);

        // Assert
        inventory.QuantityOnHand.Should().Be(70);
 }

    [Fact]
    public void RemoveStock_ShouldDecreaseReservedQuantity_WhenRemoveFromReservedIsTrue()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };
        inventory.ReserveStock(50);

        // Act
inventory.RemoveStock(30, removeFromReserved: true);

    // Assert
        inventory.QuantityOnHand.Should().Be(70);
        inventory.ReservedQuantity.Should().Be(20);
    }

    [Fact]
    public void RemoveStock_ShouldNotDecreaseReservedQuantity_WhenRemoveFromReservedIsFalse()
    {
    // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };
        inventory.ReserveStock(50);

        // Act
        inventory.RemoveStock(30, removeFromReserved: false);

        // Assert
        inventory.QuantityOnHand.Should().Be(70);
        inventory.ReservedQuantity.Should().Be(50);
    }

    [Fact]
    public void RemoveStock_ShouldThrowException_WhenQuantityIsNegative()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

        // Act
        Action act = () => inventory.RemoveStock(-10);

        // Assert
        act.Should().Throw<ArgumentException>()
 .WithMessage("*must be positive*");
    }

    [Fact]
    public void RemoveStock_ShouldThrowException_WhenQuantityIsZero()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

        // Act
        Action act = () => inventory.RemoveStock(0);

        // Assert
        act.Should().Throw<ArgumentException>()
        .WithMessage("*must be positive*");
    }

    [Fact]
    public void RemoveStock_ShouldThrowException_WhenInsufficientStock()
    {
        // Arrange
   var inventory = new Inventory { QuantityOnHand = 50 };

        // Act
    Action act = () => inventory.RemoveStock(60);

        // Assert
        act.Should().Throw<InvalidOperationException>()
        .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public void RemoveStock_ShouldNotReduceReservedQuantity_WhenReservedIsLessThanRemovalAmount()
    {
        // Arrange
 var inventory = new Inventory { QuantityOnHand = 100 };
        inventory.ReserveStock(20);

 // Act
    inventory.RemoveStock(50, removeFromReserved: true);

        // Assert
        inventory.QuantityOnHand.Should().Be(50);
        inventory.ReservedQuantity.Should().Be(20); // Should not go negative
    }

    #endregion

    #region Validate Tests

    [Fact]
    public void Validate_ShouldReturnTrue_WhenInventoryIsValid()
  {
        // Arrange
        var inventory = new Inventory
{
            QuantityOnHand = 100,
 ReorderLevel = 10,
    MaxStockLevel = 200
      };
        inventory.ReserveStock(30);

        // Act & Assert
        inventory.Validate().Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenQuantityOnHandIsNegative()
    {
 // Arrange
  var inventory = new Inventory
        {
         ReorderLevel = 10,
         MaxStockLevel = 200
};
        // Using reflection to bypass property validation for testing
   typeof(Inventory)
          .GetField("_quantityOnHand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
  .SetValue(inventory, -5);

        // Act & Assert
        inventory.Validate().Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnFalse_WhenMaxStockLevelIsLessThanReorderLevel()
    {
  // Arrange
        var inventory = new Inventory
        {
 QuantityOnHand = 100
    };
        // Set reorder level first, then try to manipulate max stock level
        typeof(Inventory)
            .GetField("_reorderLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(inventory, 100);
      typeof(Inventory)
            .GetField("_maxStockLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
      .SetValue(inventory, 50);

   // Act & Assert
        inventory.Validate().Should().BeFalse();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void CompleteOrderWorkflow_ShouldWorkCorrectly()
    {
        // Arrange - Start with 100 items in stock
        var inventory = new Inventory
        {
       QuantityOnHand = 100,
         ReorderLevel = 20,
     MaxStockLevel = 150
   };

        // Act & Assert - Customer orders 30 items
        inventory.ReserveStock(30);
        inventory.ReservedQuantity.Should().Be(30);
        inventory.AvailableQuantity.Should().Be(70);

        // Order is fulfilled - remove the 30 items
        inventory.RemoveStock(30);
        inventory.QuantityOnHand.Should().Be(70);
        inventory.ReservedQuantity.Should().Be(0);
        inventory.AvailableQuantity.Should().Be(70);
}

    [Fact]
  public void CancelledOrderWorkflow_ShouldWorkCorrectly()
    {
 // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

    // Act - Reserve stock for order
        inventory.ReserveStock(40);
        inventory.ReservedQuantity.Should().Be(40);
        inventory.AvailableQuantity.Should().Be(60);

        // Order is cancelled - release the reserved stock
      inventory.ReleaseStock(40);
        inventory.ReservedQuantity.Should().Be(0);
  inventory.AvailableQuantity.Should().Be(100);
        inventory.QuantityOnHand.Should().Be(100);
    }

    [Fact]
    public void RestockWorkflow_ShouldWorkCorrectly()
    {
        // Arrange - Low stock situation
     var inventory = new Inventory
        {
      QuantityOnHand = 15,
         ReorderLevel = 20,
  MaxStockLevel = 100
        };

        // Act & Assert - Check if reorder is needed
      inventory.NeedsReorder.Should().BeTrue();

  // New shipment arrives
        inventory.AddStock(50);
  inventory.QuantityOnHand.Should().Be(65);
        inventory.NeedsReorder.Should().BeFalse();
    }

    [Fact]
 public void MultipleSimultaneousOrders_ShouldBeHandledCorrectly()
    {
        // Arrange
        var inventory = new Inventory { QuantityOnHand = 100 };

        // Act - Multiple customers order simultaneously
        inventory.ReserveStock(20); // Order 1
        inventory.ReserveStock(30); // Order 2
        inventory.ReserveStock(15); // Order 3

        // Assert
      inventory.ReservedQuantity.Should().Be(65);
 inventory.AvailableQuantity.Should().Be(35);
        
        // One order is fulfilled
        inventory.RemoveStock(20);
      inventory.QuantityOnHand.Should().Be(80);
  inventory.ReservedQuantity.Should().Be(45);
   inventory.AvailableQuantity.Should().Be(35);
    }

 #endregion

    #region Edge Cases Tests

    [Fact]
    public void Inventory_ShouldHandleZeroStock()
  {
        // Arrange
   var inventory = new Inventory { QuantityOnHand = 0 };

    // Assert
        inventory.IsInStock.Should().BeFalse();
        inventory.AvailableQuantity.Should().Be(0);
        inventory.NeedsReorder.Should().BeTrue();
    }

    [Fact]
    public void Inventory_ShouldHandleExactReorderLevel()
    {
        // Arrange
        var inventory = new Inventory
        {
       QuantityOnHand = 5,
            ReorderLevel = 5
 };

        // Assert
        inventory.NeedsReorder.Should().BeTrue();
    }

    [Fact]
    public void Inventory_ShouldAllowReservingAllAvailableStock()
    {
 // Arrange
        var inventory = new Inventory { QuantityOnHand = 50 };

   // Act
        inventory.ReserveStock(50);

        // Assert
        inventory.ReservedQuantity.Should().Be(50);
        inventory.AvailableQuantity.Should().Be(0);
        inventory.IsInStock.Should().BeFalse();
    }

    #endregion
}
