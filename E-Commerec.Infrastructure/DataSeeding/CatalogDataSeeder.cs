using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Order;
using E_Commerce.Domain.Entities.Product;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerec.Infrastructure.DataSeeding
{
    public class CatalogDataSeeder(StoreDbContext storeDbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedAsync(CancellationToken ct = default)
        {
            try {

                var pending = await storeDbContext.Database.GetPendingMigrationsAsync();
                if (pending.Count() > 0) await storeDbContext.Database.MigrateAsync();

                var seedPath = Path.Combine(AppContext.BaseDirectory, "DataSeed");

                await SeedIfEmpty<ProductBrand>(seedPath, "brands.json", ct);
                await SeedIfEmpty<ProductType>(seedPath, "types.json", ct);
                await SeedIfEmpty<Product>(seedPath, "products.json", ct);
                await SeedIfEmpty<DeliveryMethod>(seedPath, "delivery.json", ct);


            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "failed to seed data");
                throw;
            }
            }
        private async Task SeedIfEmpty<T>(string root ,string fileName , CancellationToken ct) where T : class
        {
            if (await storeDbContext.Set<T>().AnyAsync(ct)) return;

            var filePath = Path.Combine(root , fileName);

            if (!File.Exists(filePath))
            {
                logger.LogWarning($"seed file not found {filePath}");
                return;
            }

            await using var stream = File.OpenRead(filePath);

            var items = await JsonSerializer.DeserializeAsync<List<T>>(stream , new JsonSerializerOptions { PropertyNameCaseInsensitive = true } , ct);

            if(items?.Count() > 0) await storeDbContext.AddRangeAsync(items, ct);

            await storeDbContext.SaveChangesAsync(ct);
        }
    }
}
