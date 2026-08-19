# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["InventoryStockApi.Api/InventoryStockApi.Api.csproj", "InventoryStockApi.Api/"]
RUN dotnet restore "InventoryStockApi.Api/InventoryStockApi.Api.csproj"

COPY . .
WORKDIR /src/InventoryStockApi.Api
RUN dotnet build "InventoryStockApi.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "InventoryStockApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "InventoryStockApi.Api.dll"]