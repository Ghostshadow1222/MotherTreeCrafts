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
    /// Material or filament type (e.g., PLA, ABS, PETG, resin, wood, fabric, yarn)
    /// </summary>
    [MaxLength(100)]
    public string? Material { get; set; }

    /// <summary>
    /// Dimensions of the product (e.g., "10cm x 5cm x 3cm" or "Small/Medium/Large")
    /// </summary>
    [MaxLength(100)]
    public string? Dimensions { get; set; }

    /// <summary>
    /// Weight of the product in grams (for shipping calculations)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? WeightInGrams { get; set; }

    /// <summary>
    /// Indicates if the product can be customized by customers
    /// </summary>
    public bool IsCustomizable { get; set; } = false;

    /// <summary>
    /// Indicates if this is a digital product (e.g., STL files, patterns, templates)
    /// </summary>
    public bool IsDigitalProduct { get; set; } = false;

    /// <summary>
    /// Comma-separated tags for better categorization and search (e.g., "3d-print,miniature,gaming")
    /// </summary>
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// Indicates if the product is active and visible in the store
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates if the product should be featured on the homepage or in promotions
    /// </summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Available color options (e.g., "Red,Blue,Green,Custom")
    /// </summary>
    [MaxLength(200)]
    public string? ColorOptions { get; set; }

    /// <summary>
    /// Difficulty level for craft kits or patterns (e.g., "Beginner", "Intermediate", "Advanced")
    /// </summary>
    [MaxLength(50)]
    public string? DifficultyLevel { get; set; }

    /// <summary>
    /// URL or file path for downloadable digital content (for digital products)
    /// </summary>
    [MaxLength(500)]
    public string? DigitalFileUrl { get; set; }

    /// <summary>
    /// Designer or artist name (optional attribution)
    /// </summary>
    [MaxLength(200)]
    public string? DesignerName { get; set; }

    /// <summary>
    /// Indicates if this is a made-to-order item
    /// </summary>
    public bool IsMadeToOrder { get; set; } = false;

    /// <summary>
    /// Total quantity of the product available in stock (computed from Inventory)
    /// </summary>
    [NotMapped]
    public int StockQuantity => Inventory?.AvailableQuantity ?? 0;

    /// <summary>
    /// Average rating based on all reviews (computed property)
    /// </summary>
    [NotMapped]
    public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0.0;

    /// <summary>
    /// Total number of reviews (computed property)
    /// </summary>
    [NotMapped]
    public int TotalReviews => Reviews.Count;

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
