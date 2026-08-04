using Microsoft.EntityFrameworkCore;
using InventoryStockApi.Api.Controllers;
using InventoryStockApi.Api.Data;
using InventoryStockApi.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace InventoryStockApi.Tests;

public class StockMovementsControllerTests
{
    private InventoryDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InventoryDbContext(options);
    }

    [Fact]
    public async Task GetStockLevels_CalculatesCorrectStock_WithInAndOutMovements()
    {
        var context = GetInMemoryContext();

        var product = new Product { Sku = "SKU-001", Name = "Widget", Category = "Test" };
        var warehouse = new Warehouse { Name = "Main", Location = "Cape Town" };
        context.Products.Add(product);
        context.Warehouses.Add(warehouse);
        await context.SaveChangesAsync();

        context.StockMovements.AddRange(
            new StockMovement { ProductId = product.Id, WarehouseId = warehouse.Id, Quantity = 100, MovementType = MovementType.In },
            new StockMovement { ProductId = product.Id, WarehouseId = warehouse.Id, Quantity = 30, MovementType = MovementType.Out }
        );
        await context.SaveChangesAsync();

        var controller = new StockMovementsController(context);

        var result = await controller.GetStockLevels();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var levels = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        var level = Assert.Single(levels);

        var currentStock = level.GetType().GetProperty("CurrentStock")!.GetValue(level);
        Assert.Equal(70, currentStock);
    }

    [Fact]
    public async Task GetStockLevels_ReturnsEmpty_WhenNoMovementsExist()
    {
        var context = GetInMemoryContext();
        var controller = new StockMovementsController(context);

        var result = await controller.GetStockLevels();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var levels = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        Assert.Empty(levels);
    }
}