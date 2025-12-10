using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotherTreeCrafts.Data;
using MotherTreeCrafts.Models;
using MotherTreeCrafts.Models.ViewModels;

namespace MotherTreeCrafts.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Products - Requires Authorization (temporarily disabled for testing)
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Inventory)
            .OrderByDescending(p => p.ProductId)
            .ToListAsync();
        return View(products);
    }

    // GET: Products/Details/5 - Public
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(m => m.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // GET: Products/Create - Temporarily allowing all authenticated users for testing
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create - Temporarily allowing all authenticated users for testing
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(ProductCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Create the product
                var product = new Product
                {
                    Title = model.Title,
                    Price = model.Price,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    CraftType = model.CraftType,
                    Material = model.Material,
                    Dimensions = model.Dimensions,
                    WeightInGrams = model.WeightInGrams,
                    IsCustomizable = model.IsCustomizable,
                    IsDigitalProduct = model.IsDigitalProduct,
                    Tags = model.Tags,
                    IsActive = model.IsActive,
                    IsFeatured = model.IsFeatured,
                    ColorOptions = model.ColorOptions,
                    DifficultyLevel = model.DifficultyLevel,
                    DigitalFileUrl = model.DigitalFileUrl,
                    DesignerName = model.DesignerName,
                    IsMadeToOrder = model.IsMadeToOrder
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Create initial inventory record if stock quantity is provided
                if (model.InitialStockQuantity > 0 || !string.IsNullOrWhiteSpace(model.SKU))
                {
                    var inventory = new Inventory
                    {
                        ProductId = product.ProductId,
                        QuantityOnHand = model.InitialStockQuantity,
                        ReorderLevel = model.ReorderLevel,
                        MaxStockLevel = model.MaxStockLevel,
                        SKU = model.SKU,
                        StorageLocation = model.StorageLocation
                    };

                    _context.Inventories.Add(inventory);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Product created successfully: {ProductId} - {Title}", product.ProductId, product.Title);
                TempData["SuccessMessage"] = $"Product '{product.Title}' has been created successfully!";
                
                return RedirectToAction(nameof(Details), new { id = product.ProductId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product. Please try again.");
            }
        }

        return View(model);
    }

    // GET: Products/Edit/5 - Requires Authorization
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // TODO: Implement Edit View Model and View
        return View(product);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Product updated successfully: {ProductId} - {Title}", product.ProductId, product.Title);
                TempData["SuccessMessage"] = $"Product '{product.Title}' has been updated successfully!";
                
                return RedirectToAction(nameof(Details), new { id = product.ProductId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product. Please try again.");
            }
        }

        return View(product);
    }

    // GET: Products/Delete/5 - Requires Authorization
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(m => m.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Product deleted successfully: {ProductId} - {Title}", product.ProductId, product.Title);
                TempData["SuccessMessage"] = $"Product '{product.Title}' has been deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            TempData["ErrorMessage"] = "An error occurred while deleting the product. Please try again.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }
}
