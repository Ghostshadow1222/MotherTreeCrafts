using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents detailed inventory tracking for a product
/// </summary>
public class Inventory
{
    // Constants for default values - easier to maintain and test
    public const int DefaultReorderLevel = 5;
    public const int DefaultMaxStockLevel = 100;
    public const int DefaultReservedQuantity = 0;

    [Key]
    public int InventoryId { get; set; }

    /// <summary>
    /// Foreign key to the Product being tracked
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    /// Navigation property to the associated Product
    /// </summary>
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    private int _quantityOnHand;
    /// <summary>
    /// Current quantity of the product on hand
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityOnHand
    {
        get => _quantityOnHand;
        set
        {
            if (value < 0)
                throw new ArgumentException("Quantity on hand cannot be negative.", nameof(QuantityOnHand));
            if (value < ReservedQuantity)
                throw new ArgumentException("Quantity on hand cannot be less than reserved quantity.", nameof(QuantityOnHand));
            _quantityOnHand = value;
            UpdateLastModified();
        }
    }

    private int _reorderLevel = DefaultReorderLevel;
    /// <summary>
    /// Minimum quantity before reorder is needed
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ReorderLevel
    {
        get => _reorderLevel;
        set
        {
            if (value < 0)
                throw new ArgumentException("Reorder level cannot be negative.", nameof(ReorderLevel));
            _reorderLevel = value;
            UpdateLastModified();
        }
    }

    private int _maxStockLevel = DefaultMaxStockLevel;
    /// <summary>
    /// Maximum quantity to keep in stock
    /// </summary>
    [Range(0, int.MaxValue)]
    public int MaxStockLevel
    {
        get => _maxStockLevel;
        set
        {
            if (value < 0)
                throw new ArgumentException("Max stock level cannot be negative.", nameof(MaxStockLevel));
            if (value < ReorderLevel)
                throw new ArgumentException("Max stock level cannot be less than reorder level.", nameof(MaxStockLevel));
            _maxStockLevel = value;
            UpdateLastModified();
        }
    }

    private int _reservedQuantity = DefaultReservedQuantity;
    /// <summary>
    /// Quantity currently reserved for pending orders
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ReservedQuantity
    {
        get => _reservedQuantity;
        private set // Made private - use ReserveStock/ReleaseStock methods instead
        {
            if (value < 0)
                throw new ArgumentException("Reserved quantity cannot be negative.", nameof(ReservedQuantity));
            if (value > QuantityOnHand)
                throw new InvalidOperationException("Cannot reserve more than available quantity.");
            _reservedQuantity = value;
            UpdateLastModified();
        }
    }

    /// <summary>
    /// Available quantity (QuantityOnHand - ReservedQuantity)
    /// </summary>
    [NotMapped]
    public int AvailableQuantity => QuantityOnHand - ReservedQuantity;

    /// <summary>
    /// Indicates if the product is currently in stock
    /// </summary>
    [NotMapped]
    public bool IsInStock => AvailableQuantity > 0;

    /// <summary>
    /// Indicates if inventory is below reorder level
    /// </summary>
    [NotMapped]
    public bool NeedsReorder => QuantityOnHand <= ReorderLevel;

    /// <summary>
    /// Location in warehouse/storage (e.g., "Shelf A-3", "Room 2")
    /// </summary>
    [MaxLength(100)]
    public string? StorageLocation { get; set; }

    /// <summary>
    /// SKU (Stock Keeping Unit) identifier
    /// </summary>
    [MaxLength(50)]
    public string? SKU { get; set; }

    /// <summary>
    /// Date and time of last inventory update
    /// </summary>
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Notes about the inventory (e.g., "Seasonal item", "Handmade - limited quantity")
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Business logic methods for better encapsulation

    /// <summary>
    /// Reserves a quantity of stock for an order
    /// </summary>
    /// <param name="quantity">Amount to reserve</param>
    /// <exception cref="InvalidOperationException">Thrown when there is insufficient stock</exception>
    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {AvailableQuantity}, Requested: {quantity}");

        ReservedQuantity += quantity;
    }

    /// <summary>
    /// Releases reserved stock (e.g., when an order is cancelled)
    /// </summary>
    /// <param name="quantity">Amount to release</param>
    /// <exception cref="ArgumentException">Thrown when quantity is not positive</exception>
    /// <exception cref="InvalidOperationException">Thrown when releasing more than reserved</exception>
    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException($"Cannot release more than reserved. Reserved: {ReservedQuantity}, Requested: {quantity}");

        ReservedQuantity -= quantity;
    }

    /// <summary>
    /// Adds stock to inventory (e.g., when receiving new shipment)
    /// </summary>
    /// <param name="quantity">Amount to add</param>
    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        QuantityOnHand += quantity;
    }

    /// <summary>
    /// Removes stock from inventory (e.g., when order is fulfilled)
    /// </summary>
    /// <param name="quantity">Amount to remove</param>
    /// <param name="removeFromReserved">Whether to also reduce reserved quantity</param>
    public void RemoveStock(int quantity, bool removeFromReserved = true)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        if (QuantityOnHand < quantity)
            throw new InvalidOperationException($"Insufficient stock. On hand: {QuantityOnHand}, Requested: {quantity}");

        QuantityOnHand -= quantity;

        if (removeFromReserved)
        {
            ReservedQuantity = Math.Max(0, ReservedQuantity - quantity);
        }
    }

    /// <summary>
    /// Validates the current inventory state
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool Validate()
    {
        return QuantityOnHand >= 0
            && ReservedQuantity >= 0
            && ReservedQuantity <= QuantityOnHand
            && ReorderLevel >= 0
            && MaxStockLevel >= ReorderLevel;
    }

    /// <summary>
    /// Updates the LastUpdated timestamp
    /// </summary>
    private void UpdateLastModified()
    {
        LastUpdated = DateTime.UtcNow;
    }
}
