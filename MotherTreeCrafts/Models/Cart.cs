using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a shopping cart for a user or guest session
/// </summary>
public class Cart
{
    [Key]
    public int CartId { get; set; }

    /// <summary>
    /// Foreign key to the UserAccount (nullable for guest users)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Navigation property to the UserAccount
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public UserAccount? User { get; set; }

    /// <summary>
    /// Session ID for guest users before login
    /// </summary>
    [MaxLength(200)]
    public string? SessionId { get; set; }

    /// <summary>
    /// Date and time when the cart was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the cart was last updated
    /// </summary>
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the cart expires (for auto-clearing abandoned carts)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Indicates if the cart is active (becomes false after checkout completes)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Calculated subtotal of all items in the cart
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Collection of items in this cart
    /// </summary>
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
