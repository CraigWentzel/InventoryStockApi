using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryStockApi.Api.Data;

public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseSqlServer("Server=CRAIG-WENTZEL\\SQLEXPRESS;Database=InventoryStockDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new InventoryDbContext(optionsBuilder.Options);
    }
}