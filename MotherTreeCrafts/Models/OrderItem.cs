using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents an individual item within an order
/// </summary>
public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    /// <summary>
    /// Foreign key to the Order
    /// </summary>
    [Required]
    public int OrderId { get; set; }

    /// <summary>
    /// Navigation property to the Order
    /// </summary>
    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    /// <summary>
    /// Foreign key to the Product
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    /// Navigation property to the Product
    /// </summary>
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    /// <summary>
    /// Quantity ordered
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price at the time of order (preserves price history)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Line total (Quantity * UnitPrice)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }

    /// <summary>
    /// Customization details for this order item
    /// </summary>
    [MaxLength(1000)]
    public string? CustomizationDetails { get; set; }
}