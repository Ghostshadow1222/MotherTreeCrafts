using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotherTreeCrafts.Models;

/// <summary>
/// Represents a customer order
/// </summary>
public class Order
{
    [Key]
    public int OrderId { get; set; }

    /// <summary>
    /// User-friendly order reference number (e.g., "ORD-2026-001")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string OrderNumber { get; set; }

    /// <summary>
    /// Foreign key to the UserAccount who placed the order
    /// </summary>
    [Required]
    public required string UserId { get; set; }

    /// <summary>
    /// Navigation property to the UserAccount
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public UserAccount? User { get; set; }

    /// <summary>
    /// Current status of the order
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string OrderStatus { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled, Refunded

    /// <summary>
    /// Date and time when the order was placed
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the order was last updated
    /// </summary>
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Shipping address for this order
    /// </summary>
    [Required]
    [MaxLength(500)]
    public required string ShippingAddress { get; set; }

    /// <summary>
    /// Billing address for this order
    /// </summary>
    [Required]
    [MaxLength(500)]
    public required string BillingAddress { get; set; }

    /// <summary>
    /// Subtotal before tax and shipping
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Tax amount calculated for this order
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Shipping cost for this order
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Total amount including tax and shipping
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Payment method used (e.g., "Credit Card", "PayPal")
    /// </summary>
    [MaxLength(100)]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Payment status (e.g., "Pending", "Paid", "Failed", "Refunded")
    /// </summary>
    [MaxLength(50)]
    public string PaymentStatus { get; set; } = "Pending";

    /// <summary>
    /// Tracking number for shipment
    /// </summary>
    [MaxLength(200)]
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// Date when the order was shipped
    /// </summary>
    public DateTime? ShippedDate { get; set; }

    /// <summary>
    /// Date when the order was delivered
    /// </summary>
    public DateTime? DeliveredDate { get; set; }

    /// <summary>
    /// Reason for cancellation if order was cancelled
    /// </summary>
    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Amount refunded if applicable
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? RefundAmount { get; set; }

    /// <summary>
    /// Additional notes about the order (admin or customer notes)
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Collection of order items in this order
    /// </summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
