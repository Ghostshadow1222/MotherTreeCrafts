using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents an individual item within a shopping cart
/// </summary>
public class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    /// <summary>
    /// Foreign key to the Cart
    /// </summary>
    [Required]
    public int CartId { get; set; }

    /// <summary>
    /// Navigation property to the Cart
    /// </summary>
    [ForeignKey(nameof(CartId))]
    public Cart? Cart { get; set; }

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
    /// Quantity of this product in the cart
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Price of the product at the time it was added to the cart
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceAtTimeOfAdd { get; set; }

    /// <summary>
    /// Optional customization details for this item
    /// </summary>
    [MaxLength(1000)]
    public string? CustomizationDetails { get; set; }

    /// <summary>
    /// Date and time when this item was added to the cart
    /// </summary>
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}