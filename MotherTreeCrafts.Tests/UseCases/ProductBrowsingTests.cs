using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MotherTreeCrafts.Controllers;
using MotherTreeCrafts.Data;
using MotherTreeCrafts.Models;
using Xunit;

namespace MotherTreeCrafts.Tests.UseCases;

/// <summary>
/// Unit tests for UC1: Customer browses products
/// Tests the product browsing functionality including viewing catalog and product details
/// </summary>
public class ProductBrowsingTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    #region Success Path Tests

    [Fact]
    public async Task Index_ShouldReturnAllActiveProducts()
    {
        // Arrange
        var context = GetInMemoryContext();
        var mockLogger = new Mock<ILogger<ProductsController>>();
        var controller = new ProductsController(context, mockLogger.Object);

        var products = new List<Product>
        {
            new Product { ProductId = 1, Title = "Product 1", Price = 10.00m, IsActive = true, Description = "Test" },
            new Product { ProductId = 2, Title = "Product 2", Price = 20.00m, IsActive = true, Description = "Test" },
            new Product { ProductId = 3, Title = "Product 3", Price = 30.00m, IsActive = true, Description = "Test" }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<List<Product>>().Subject;
        model.Should().HaveCount(3);
        model.Should().Contain(p => p.Title == "Product 1");
    }

    [Fact]
    public async Task Details_ShouldReturnProductWithInventoryAndReviews()
    {
        // Arrange
        var context = GetInMemoryContext();
        var mockLogger = new Mock<ILogger<ProductsController>>();
        var controller = new ProductsController(context, mockLogger.Object);

        var product = new Product
        {
            ProductId = 1,
            Title = "Test Product",
            Price = 10.00m,
            IsActive = true,
            Description = "Test Description"
        };

        var inventory = new Inventory
        {
            InventoryId = 1,
            ProductId = 1,
            QuantityOnHand = 50
        };

        await context.Products.AddAsync(product);
        await context.Inventories.AddAsync(inventory);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<Product>().Subject;
        model.Title.Should().Be("Test Product");
        model.Inventory.Should().NotBeNull();
        model.Inventory!.QuantityOnHand.Should().Be(50);
    }

    #endregion

    #region Alternate Flow Tests

    [Fact]
    public async Task Index_ShouldReturnEmptyList_WhenNoCatalogProducts()
    {
        // Arrange
        var context = GetInMemoryContext();
        var mockLogger = new Mock<ILogger<ProductsController>>();
        var controller = new ProductsController(context, mockLogger.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<List<Product>>().Subject;
        model.Should().BeEmpty();
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenProductIdIsNull()
    {
        // Arrange
        var context = GetInMemoryContext();
        var mockLogger = new Mock<ILogger<ProductsController>>();
        var controller = new ProductsController(context, mockLogger.Object);

        // Act
        var result = await controller.Details(null);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryContext();
        var mockLogger = new Mock<ILogger<ProductsController>>();
        var controller = new ProductsController(context, mockLogger.Object);

        // Act
        var result = await controller.Details(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Details_ShouldDisplayProduct_WhenOutOfStock()
    {
        // Arrange
        var context = GetInMemoryContext();
        var mockLogger = new Mock<ILogger<ProductsController>>();
        var controller = new ProductsController(context, mockLogger.Object);

        var product = new Product
        {
            ProductId = 1,
            Title = "Out of Stock Product",
            Price = 10.00m,
            IsActive = true,
            Description = "Test"
        };

        var inventory = new Inventory
        {
            InventoryId = 1,
            ProductId = 1,
            QuantityOnHand = 0
        };

        await context.Products.AddAsync(product);
        await context.Inventories.AddAsync(inventory);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<Product>().Subject;
        model.Inventory!.QuantityOnHand.Should().Be(0);
    }

    #endregion
}