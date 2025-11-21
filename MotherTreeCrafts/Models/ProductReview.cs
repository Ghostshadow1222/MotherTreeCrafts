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
  /// User ID of the reviewer (optional - for authenticated users)
  /// </summary>
  [MaxLength(450)]
  public string? UserId { get; set; }

  /// <summary>
  /// Name of the reviewer
  /// </summary>
  [Required]
  [MaxLength(100)]
  public required string ReviewerName { get; set; }

  /// <summary>
  /// Email of the reviewer (optional, for verification purposes)
  /// </summary>
  [MaxLength(200)]
  [EmailAddress]
  public string? ReviewerEmail { get; set; }

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
  /// URLs or paths to images uploaded by the reviewer (comma-separated)
  /// </summary>
  [MaxLength(1000)]
  public string? ReviewImages { get; set; }

  /// <summary>
  /// Date and time when the review was created
  /// </summary>
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public DateTime ReviewDate { get; set; }

  /// <summary>
  /// Date and time when the review was last updated/edited
  /// </summary>
  public DateTime? UpdatedDate { get; set; }

  /// <summary>
  /// Whether the review has been verified (e.g., verified purchase)
  /// </summary>
  public bool IsVerified { get; set; } = false;

  /// <summary>
  /// Whether the reviewer actually purchased this product
  /// </summary>
  public bool IsPurchaseVerified { get; set; } = false;

  /// <summary>
  /// Whether the review has been approved by admin/moderator
  /// </summary>
  public bool IsApproved { get; set; } = true;

  /// <summary>
  /// Whether the review is currently visible to customers
  /// </summary>
  public bool IsVisible { get; set; } = true;

  /// <summary>
  /// Number of helpful votes this review received
  /// </summary>
  public int HelpfulCount { get; set; } = 0;

  /// <summary>
  /// Number of times this review was reported as inappropriate
  /// </summary>
  public int ReportedCount { get; set; } = 0;

  /// <summary>
  /// Admin or store owner's response to this review
  /// </summary>
  [MaxLength(1000)]
  public string? AdminResponse { get; set; }

  /// <summary>
  /// Date when admin responded
  /// </summary>
  public DateTime? AdminResponseDate { get; set; }

  /// <summary>
  /// Internal notes about this review (not visible to customers)
  /// </summary>
  [MaxLength(500)]
  public string? ModeratorNotes { get; set; }

  /// <summary>
  /// Indicates if the reviewer would recommend this product
  /// </summary>
  public bool? WouldRecommend { get; set; }
}
