using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Identity;
using E_Commerec.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace E_Commerec.Infrastructure.DataSeeding
{
    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreIdentityDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<IdentityDataSeeder> logger;
        private readonly IConfiguration configuration;
        private readonly IHostEnvironment environment;

        public IdentityDataSeeder(StoreIdentityDbContext dbContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ILogger<IdentityDataSeeder> logger, IConfiguration configuration,
            IHostEnvironment environment)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this.configuration = configuration;
            this.environment = environment;
        }
        public async Task SeedAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any()) await dbContext.Database.MigrateAsync(ct);

                if (!await roleManager.Roles.AnyAsync(ct))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }

                if (environment.IsDevelopment())
                {
                    var email = configuration["SeedAdmin:Email"];
                    var password = configuration["SeedAdmin:Password"];
                    var displayName = configuration["SeedAdmin:DisplayName"];

                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    {
                        logger.LogWarning(
                            "Skipping test admin creation: SeedAdmin settings are missing.");
                    }
                    else if (await userManager.FindByEmailAsync(email) is null)
                    {
                        var admin = new ApplicationUser
                        {
                            DisplayName = displayName ?? "Test Admin",
                            Email = email,
                            UserName = email
                        };

                        var result = await userManager.CreateAsync(admin, password);

                        if (result.Succeeded)
                        {
                            var roleResult =
                                await userManager.AddToRoleAsync(admin, "Admin");

                            if (!roleResult.Succeeded)
                            {
                                throw new InvalidOperationException(
                                    "Failed to assign the Admin role.");
                            }
                        }
                        else
                        {
                            logger.LogWarning("Failed to create the test admin.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding identity data.");
                throw;
            }
        }
    }
}
