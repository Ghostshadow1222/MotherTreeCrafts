using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotherTreeCrafts.Data;
using MotherTreeCrafts.Models;

namespace MotherTreeCrafts.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserAccount> _userManager;
    private readonly SignInManager<UserAccount> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        ApplicationDbContext context,
        UserManager<UserAccount> userManager,
        SignInManager<UserAccount> signInManager,
        ILogger<AccountController> logger)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var wishlistItems = await _context.Wishlists
            .Where(w => w.AccountId == user.Id)
            .Include(w => w.Product)
            .ThenInclude(p => p.Inventory)
            .OrderBy(w => w.Priority)
            .ThenByDescending(w => w.DateAdded)
            .ToListAsync();

        ViewData["User"] = user;
        ViewData["WishlistItems"] = wishlistItems;

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(UserAccount model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        if (ModelState.IsValid)
        {
            try
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.ShippingAddress = model.ShippingAddress;
                user.BillingAddress = model.BillingAddress;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                ModelState.AddModelError(string.Empty, "An error occurred while updating your profile.");
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToWishlist(int productId, int priority = 3)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var existingItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.AccountId == user.Id && w.ProductId == productId);

            if (existingItem != null)
            {
                return Json(new { success = false, message = "Item already in wishlist" });
            }

            var wishlistItem = new Wishlist
            {
                AccountId = user.Id,
                ProductId = productId,
                Priority = priority
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} added to wishlist for user {UserId}", productId, user.Id);
            return Json(new { success = true, message = "Added to wishlist!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product to wishlist");
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        try
        {
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.WishlistId == wishlistId && w.AccountId == user.Id);

            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Item removed from wishlist.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from wishlist");
            TempData["ErrorMessage"] = "An error occurred while removing the item.";
        }

        return RedirectToAction(nameof(Index));
    }
}
