using Microsoft.EntityFrameworkCore;
using InventoryStockApi.Api.Controllers;
using InventoryStockApi.Api.Data;
using InventoryStockApi.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace InventoryStockApi.Tests;

public class ProductsControllerTests
{
    private InventoryDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InventoryDbContext(options);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmptyList_WhenNoProductsExist()
    {
        var context = GetInMemoryContext();
        var controller = new ProductsController(context);

        var result = await controller.GetProducts();

        var okResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Empty(products);
    }

    [Fact]
    public async Task CreateProduct_AddsProductToDatabase()
    {
        var context = GetInMemoryContext();
        var controller = new ProductsController(context);
        var newProduct = new Product { Sku = "TEST-001", Name = "Test Widget", Category = "General" };

        var result = await controller.CreateProduct(newProduct);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var product = Assert.IsType<Product>(createdResult.Value);
        Assert.Equal("TEST-001", product.Sku);
        Assert.Single(context.Products);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var context = GetInMemoryContext();
        var controller = new ProductsController(context);

        var result = await controller.GetProduct(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteProduct_RemovesProductFromDatabase()
    {
        var context = GetInMemoryContext();
        context.Products.Add(new Product { Sku = "DEL-001", Name = "To Delete", Category = "Test" });
        await context.SaveChangesAsync();
        var controller = new ProductsController(context);

        var result = await controller.DeleteProduct(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Products);
    }
}