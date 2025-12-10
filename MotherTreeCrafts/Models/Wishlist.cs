using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a user's wishlist item - linking an account to a saved product
/// </summary>
public class Wishlist
{
  [Key]
  public int WishlistId { get; set; }

  /// <summary>
  /// Foreign key to the Account/User who saved this product
  /// </summary>
  [Required]
  public required string AccountId { get; set; }

  /// <summary>
  /// Navigation property to the UserAccount
  /// </summary>
  [ForeignKey(nameof(AccountId))]
  public UserAccount? Account { get; set; }

  /// <summary>
  /// Foreign key to the Product that was saved
  /// </summary>
  [Required]
  public int ProductId { get; set; }

  /// <summary>
  /// Navigation property to the Product
  /// </summary>
  [ForeignKey(nameof(ProductId))]
  public Product? Product { get; set; }

  /// <summary>
  /// Date and time when the product was added to the wishlist
  /// </summary>
  /// Automatically set to the current UTC time when a new Wishlist is created
  public DateTime DateAdded { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// Optional notes about why the user saved this item
  /// </summary>
  [MaxLength(500)]
  public string? Notes { get; set; }

  /// <summary>
  /// Priority level for this wishlist item (1 = highest priority)
  /// </summary>
  [Required]
  [Range(1, 5)]
  public int Priority { get; set; }
}
