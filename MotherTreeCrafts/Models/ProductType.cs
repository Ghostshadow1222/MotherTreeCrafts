using System.ComponentModel.DataAnnotations;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a category or classification for products (e.g., Crochet, Woodwork, Jewelry)
/// </summary>
public class ProductType
{
    [Key]
    public int ProductTypeId { get; set; }

    /// <summary>
    /// Name of the product type (e.g., "3D Printing", "Knitting", "Pottery", "Woodworking")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Optional description of this product type
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /* Brought up by copilot, but not sure if we need this right now, will review with client
        /// <summary>
        /// URL-friendly version of the name for routing (e.g., "handmade-jewelry")
        /// </summary>
        [MaxLength(150)]
        public string? Slug { get; set; }
    */

    /// <summary>
    /// Display order for sorting categories (lower numbers appear first)
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Collection of products belonging to this type
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
