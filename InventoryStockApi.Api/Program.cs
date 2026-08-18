using Microsoft.EntityFrameworkCore;
using InventoryStockApi.Api.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Apply pending EF Core migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    db.Database.Migrate();
}

// Swagger always enabled (Development + Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryStockApi v1");
    c.RoutePrefix = "swagger"; // UI available at /swagger/index.html
});

// Remove HTTPS redirection for Render (proxy already handles HTTPS)
// app.UseHttpsRedirection();

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Add a friendly root endpoint
app.MapGet("/", () => "InventoryStockApi is running. Visit /swagger for API docs.");

app.Run();
