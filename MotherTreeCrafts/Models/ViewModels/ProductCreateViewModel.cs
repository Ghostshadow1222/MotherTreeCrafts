using System.ComponentModel.DataAnnotations;

namespace MotherTreeCrafts.Models.ViewModels;

/// <summary>
/// View model for creating a new product
/// </summary>
public class ProductCreateViewModel
{
    [Required(ErrorMessage = "Product title is required")]
    [Display(Name = "Product Title")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Display(Name = "Price")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Display(Name = "Description")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "Image URL")]
    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    [DataType(DataType.ImageUrl)]
    public string? ImageUrl { get; set; }

    [Display(Name = "Craft Type")]
    [StringLength(100, ErrorMessage = "Craft type cannot exceed 100 characters")]
    public string? CraftType { get; set; }

    [Display(Name = "Material")]
    [StringLength(100, ErrorMessage = "Material cannot exceed 100 characters")]
    public string? Material { get; set; }

    [Display(Name = "Dimensions")]
    [StringLength(100, ErrorMessage = "Dimensions cannot exceed 100 characters")]
    public string? Dimensions { get; set; }

    [Display(Name = "Weight (grams)")]
    [Range(0, 999999, ErrorMessage = "Weight must be between 0 and 999,999 grams")]
    public decimal? WeightInGrams { get; set; }

    [Display(Name = "Is Customizable?")]
    public bool IsCustomizable { get; set; }

    [Display(Name = "Is Digital Product?")]
    public bool IsDigitalProduct { get; set; }

    [Display(Name = "Tags (comma-separated)")]
    [StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
    public string? Tags { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Featured")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Color Options (comma-separated)")]
    [StringLength(200, ErrorMessage = "Color options cannot exceed 200 characters")]
    public string? ColorOptions { get; set; }

    [Display(Name = "Difficulty Level")]
    [StringLength(50, ErrorMessage = "Difficulty level cannot exceed 50 characters")]
    public string? DifficultyLevel { get; set; }

    [Display(Name = "Digital File URL")]
    [StringLength(500, ErrorMessage = "Digital file URL cannot exceed 500 characters")]
    [DataType(DataType.Url)]
    public string? DigitalFileUrl { get; set; }

    [Display(Name = "Designer Name")]
    [StringLength(200, ErrorMessage = "Designer name cannot exceed 200 characters")]
    public string? DesignerName { get; set; }

    [Display(Name = "Made to Order?")]
    public bool IsMadeToOrder { get; set; }

    // Optional: Initial inventory information
    [Display(Name = "Initial Stock Quantity")]
    [Range(0, 999999, ErrorMessage = "Quantity must be between 0 and 999,999")]
    public int InitialStockQuantity { get; set; }

    [Display(Name = "Reorder Level")]
    [Range(0, 999999, ErrorMessage = "Reorder level must be between 0 and 999,999")]
    public int ReorderLevel { get; set; } = 5;

    [Display(Name = "Max Stock Level")]
    [Range(0, 999999, ErrorMessage = "Max stock level must be between 0 and 999,999")]
    public int MaxStockLevel { get; set; } = 100;

    [Display(Name = "SKU")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string? SKU { get; set; }

    [Display(Name = "Storage Location")]
    [StringLength(100, ErrorMessage = "Storage location cannot exceed 100 characters")]
    public string? StorageLocation { get; set; }
}
