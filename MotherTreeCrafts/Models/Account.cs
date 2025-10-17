using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a user account with additional profile information
/// </summary>
/// <remarks>Use this class for additional data specific to users outside of the default identity fields</remarks>
public class Account : IdentityUser
{
    /// <summary>
    /// User's legal first name
    /// </summary>
    public required string FirstName { get; set; }
    /// <summary>
    /// User's legal last name
    /// </summary>
    public required string LastName { get; set; }
    /// <summary>
    /// User's address for sending out orders
    /// </summary>
    public required string ShippingAddress { get; set; }
    /// <summary>
    /// User's address for billing purposes
    /// </summary>
    public required string BillingAddress { get; set; }

}
