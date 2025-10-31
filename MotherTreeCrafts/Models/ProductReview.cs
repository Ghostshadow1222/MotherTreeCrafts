using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a customer review for a product
/// </summary>
public class ProductReview
{
  [Key]
  public int ReviewId { get; set; }

  /// <summary>
  /// Foreign key to the Product being reviewed
  /// </summary>
  [Required]
  public int ProductId { get; set; }

  /// <summary>
  /// Navigation property to the associated Product
  /// </summary>
  [ForeignKey(nameof(ProductId))]
  public Product? Product { get; set; }

  /// <summary>
  /// Name of the reviewer
  /// </summary>
  [Required]
  [MaxLength(100)]
  public required string ReviewerName { get; set; }

  /// <summary>
  /// Rating from 1 to 5 stars
  /// </summary>
  [Range(1, 5)]
  public int Rating { get; set; }

  /// <summary>
  /// Review title/summary
  /// </summary>
  [MaxLength(200)]
  public string? Title { get; set; }

  /// <summary>
  /// Detailed review comment
  /// </summary>
  [Required]
  [MaxLength(2000)]
  public required string Comment { get; set; }

  /// <summary>
  /// Date and time when the review was created
  /// </summary>
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public DateTime ReviewDate { get; set; }

  /// <summary>
  /// Whether the review has been verified (e.g., verified purchase)
  /// </summary>
  public bool IsVerified { get; set; } = false;

  /// <summary>
  /// Number of helpful votes this review received
  /// </summary>
  public int HelpfulCount { get; set; } = 0;
}
