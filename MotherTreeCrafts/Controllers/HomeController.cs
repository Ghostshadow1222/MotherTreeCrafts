using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotherTreeCrafts.Data;
using MotherTreeCrafts.Models;

namespace MotherTreeCrafts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Inventory)
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.ProductId)
                .ToListAsync();
            
            return View(products);
        }
        /* -- Need to refactor how category is being assigned and identifyied
        public async Task<IActionResult> Category(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return RedirectToAction(nameof(Index));
            }

            var products = await _context.Products
                .Where(p => p.IsActive && p.CraftType == category)
                .Include(p => p.Inventory)
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.ProductId)
                .ToListAsync();

            ViewData["Title"] = category;
            ViewData["Category"] = category;
            
            return View("Category", products);
        }
        */
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
