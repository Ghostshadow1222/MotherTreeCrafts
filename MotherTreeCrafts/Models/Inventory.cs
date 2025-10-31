using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents detailed inventory tracking for a product
/// </summary>
public class Inventory
{
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

    /// <summary>
    /// Current quantity of the product on hand
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityOnHand { get; set; }

    /// <summary>
    /// Minimum quantity before reorder is needed
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; } = 5;

    /// <summary>
    /// Maximum quantity to keep in stock
    /// </summary>
    [Range(0, int.MaxValue)]
    public int MaxStockLevel { get; set; } = 100;

    /// <summary>
    /// Quantity currently reserved for pending orders
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ReservedQuantity { get; set; } = 0;

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
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Notes about the inventory (e.g., "Seasonal item", "Handmade - limited quantity")
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}
