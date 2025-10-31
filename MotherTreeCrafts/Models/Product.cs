using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a product available for purchase in the Mother Tree Crafts store
/// </summary>
public class Product
{
    [Key]
    public int ProductId { get; set; }

    /// <summary>
    /// Title of the product
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Small description of the product
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Picture/Multiple pictures of the product
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Type of craft (e.g., knitting, pottery, woodworking)
    /// </summary>
    public string? CraftType { get; set; }

    /// <summary>
    /// Total quantity of the product available in stock (computed from Inventory)
    /// </summary>
    [NotMapped]
    public int StockQuantity => Inventory?.AvailableQuantity ?? 0;

    /// <summary>
    /// Collection of reviews for this product
    /// </summary>
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();

    /// <summary>
    /// Collection of wishlist entries for this product
    /// </summary>
    public ICollection<Wishlist> WishlistEntries { get; set; } = new List<Wishlist>();

    /// <summary>
    /// Detailed inventory tracking for this product
    /// </summary>
    [ForeignKey("ProductId")]
    public Inventory? Inventory { get; set; }
}
