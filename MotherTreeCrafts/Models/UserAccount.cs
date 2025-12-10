using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a user account with additional profile information
/// </summary>
/// <remarks>Use this class for additional data specific to users outside of the default identity fields</remarks>
public class UserAccount : IdentityUser
{
    /// <summary>
    /// User's legal first name
    /// </summary>
    public string? FirstName { get; set; }
    /// <summary>
    /// User's legal last name
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// User's address for sending out orders
    /// </summary>
    public string? ShippingAddress { get; set; }
    /// <summary>
    /// User's address for billing purposes
    /// </summary>
    public string? BillingAddress { get; set; }

    /// <summary>
    /// Collection of wishlist items for this account
    /// </summary>
    public ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
}
