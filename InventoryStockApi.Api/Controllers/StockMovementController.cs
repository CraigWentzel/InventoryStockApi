using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryStockApi.Api.Data;
using InventoryStockApi.Api.Models;

namespace InventoryStockApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockMovementsController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public StockMovementsController(InventoryDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<StockMovement>> CreateMovement(StockMovement movement)
    {
        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMovement), new { id = movement.Id }, movement);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovement>> GetMovement(int id)
    {
        var movement = await _context.StockMovements.FindAsync(id);
        if (movement == null) return NotFound();
        return movement;
    }

    [HttpGet("stock-levels")]
public async Task<ActionResult<IEnumerable<object>>> GetStockLevels()
{
    var levels = await _context.StockMovements
        .GroupBy(sm => new
        {
            sm.WarehouseId,
            WarehouseName = sm.Warehouse.Name,
            sm.ProductId,
            ProductSku = sm.Product.Sku,
            ProductName = sm.Product.Name
        })
        .Select(g => new
        {
            Warehouse = g.Key.WarehouseName,
            ProductSku = g.Key.ProductSku,
            ProductName = g.Key.ProductName,
            CurrentStock = g.Sum(sm => sm.MovementType == MovementType.In ? sm.Quantity : -sm.Quantity)
        })
        .ToListAsync();

    return Ok(levels);
}
}