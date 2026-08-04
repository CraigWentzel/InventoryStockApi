using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStockApi.Api.Models;

public enum MovementType
{
    In,
    Out
}

public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Warehouse Warehouse { get; set; } = null!;

    public int Quantity { get; set; }
    public MovementType MovementType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}