using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using E_Commerec.Infrastructure.DataSeeding;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Extentions
{
    public static class ProgramExtention
    {
        public static async Task MigrationAndSeedAsync(this WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Catalog");
            await seeder.SeedAsync();

            var identitySeeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Identity");
            await identitySeeder.SeedAsync();
        }
    }
}
