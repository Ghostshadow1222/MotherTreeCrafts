using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotherTreeCrafts.Models;

namespace MotherTreeCrafts.Data
{
    public class ApplicationDbContext : IdentityDbContext<Account>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-one relationship between Product and Inventory
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add unique constraint on SKU if needed (ensures no duplicate SKUs)
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.SKU)
                .IsUnique()
                .HasFilter("[SKU] IS NOT NULL");

            // Optional: Add index on ProductId for faster lookups
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.ProductId);
        }
    }
}
