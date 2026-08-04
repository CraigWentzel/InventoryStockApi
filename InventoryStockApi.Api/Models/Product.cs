namespace InventoryStockApi.Api.Models;
 
public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
 
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
