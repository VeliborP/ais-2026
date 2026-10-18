using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Models;

namespace WebShop.DAL.Seed
{
    public static class DbSeeder
    {
        public static async Task InitializeAsync(WebShopDbContext context)
        {
            await context.Database.MigrateAsync();
            await SeedAsync(context);
        }

        public static async Task SeedAsync(WebShopDbContext context)
        {
            await SeedRolesAsync(context);
            await SeedUsersAsync(context);
            await SeedCategoriesAsync(context);
            await SeedProductsAsync(context);
        }

        private static async Task SeedRolesAsync(WebShopDbContext context)
        {
            if (await context.Roles.AnyAsync())
                return;

            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(WebShopDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

            context.Users.AddRange(
                new User
                {
                    Username = "admin",
                    Email = "admin@test.com",
                    Password = "admin123",
                    RoleId = adminRole.Id,
                    IsActive = true
                },
                new User
                {
                    Username = "user",
                    Email = "user@test.com",
                    Password = "user123",
                    RoleId = userRole.Id,
                    IsActive = true
                }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(WebShopDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            context.Categories.AddRange(
                new Category { Name = "Elektronika", Code = "CAT-001", Description = "Razne elektronske uređaje, uključujući računare, telefone i dodatke." },
                new Category { Name = "Knjige", Code = "CAT-002", Description = "Knjige svih žanrova: edukativne, beletristika i priručnici." },
                new Category { Name = "Odeća", Code = "CAT-003", Description = "Različita odeća za muškarce, žene i decu." }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(WebShopDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            var categories = await context.Categories.ToDictionaryAsync(c => c.Code, c => (int?)c.Id);

            context.Products.AddRange(
                new Product { Name = "Laptop", Code = "PROD-001", Description = "Laptop sa Intel i7 procesorom, 16GB RAM i SSD diskom.", CategoryId = categories.GetValueOrDefault("CAT-001"), Price = 1200 },
                new Product { Name = "Pametni telefon", Code = "PROD-002", Description = "Smartphone sa 6.5\" ekranom, 128GB memorije i trostrukom kamerom.", CategoryId = categories.GetValueOrDefault("CAT-001"), Price = 800 },
                new Product { Name = "ASP.NET Core knjiga", Code = "PROD-003", Description = "Detaljna knjiga o razvoju web aplikacija u ASP.NET Core.", CategoryId = categories.GetValueOrDefault("CAT-002"), Price = 35 },
                new Product { Name = "Majica", Code = "PROD-004", Description = "Pamučna majica različitih veličina i boja.", CategoryId = categories.GetValueOrDefault("CAT-003"), Price = 20 }
            );

            await context.SaveChangesAsync();
        }
    }
}
