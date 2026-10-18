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

            var adminRoleId = (await context.Roles.FirstAsync(r => r.Name == "Admin")).Id;
            var userRoleId = (await context.Roles.FirstAsync(r => r.Name == "User")).Id;

            context.Users.AddRange(
                new User { Username = "admin", Email = "admin@webshop.rs", Password = "admin123", RoleId = adminRoleId },
                new User { Username = "user", Email = "user@webshop.rs", Password = "user123", RoleId = userRoleId },
                new User { Username = "marko.petrovic", Email = "marko.petrovic@gmail.com", Password = "marko123", RoleId = userRoleId },
                new User { Username = "jelena.jovanovic", Email = "jelena.jovanovic@gmail.com", Password = "jelena123", RoleId = userRoleId },
                new User { Username = "nikola.ilic", Email = "nikola.ilic@yahoo.com", Password = "nikola123", RoleId = userRoleId }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(WebShopDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            context.Categories.AddRange(
                new Category { Code = "CAT-001", Name = "Elektronika", Description = "Laptopovi, mobilni telefoni, slušalice, pametni satovi i prateća oprema." },
                new Category { Code = "CAT-002", Name = "Knjige", Description = "Domaća i strana književnost, stručna literatura i priručnici." },
                new Category { Code = "CAT-003", Name = "Odeća i obuća", Description = "Muška i ženska odeća, obuća i jakne za svaku sezonu." },
                new Category { Code = "CAT-004", Name = "Kuća i kuhinja", Description = "Mali kućni aparati, posuđe i rasveta za dom." },
                new Category { Code = "CAT-005", Name = "Sport i rekreacija", Description = "Oprema za trening, fitnes, jogu i vožnju bicikla." }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(WebShopDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            var categories = await context.Categories.ToDictionaryAsync(c => c.Code, c => (int?)c.Id);

            Product Create(string code, string name, string description, decimal price, string categoryCode) => new Product
            {
                Code = code,
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categories.GetValueOrDefault(categoryCode),
                ImagePath = $"/images/products/{code.ToLower()}.jpg"
            };

            context.Products.AddRange(
                Create("PROD-001", "Laptop Lenovo IdeaPad Slim 5", "Laptop sa 14\" OLED ekranom, AMD Ryzen 7 procesorom, 16 GB RAM memorije i 512 GB SSD diskom. Idealan za posao i studiranje.", 89999m, "CAT-001"),
                Create("PROD-002", "Samsung Galaxy A55 5G", "Pametni telefon sa 6.6\" Super AMOLED ekranom, 8 GB RAM, 128 GB memorije i kamerom od 50 MP.", 46999m, "CAT-001"),
                Create("PROD-003", "Sony WH-1000XM5 slušalice", "Bežične slušalice sa aktivnim poništavanjem buke, do 30 sati rada baterije i brzim punjenjem.", 41990m, "CAT-001"),
                Create("PROD-004", "Xiaomi Watch 2 pametni sat", "Pametni sat sa AMOLED ekranom, ugrađenim GPS-om, merenjem pulsa i praćenjem sna.", 19990m, "CAT-001"),

                Create("PROD-005", "Na Drini ćuprija - Ivo Andrić", "Roman za koji je Ivo Andrić dobio Nobelovu nagradu. Tvrd povez, 384 strane.", 1290m, "CAT-002"),
                Create("PROD-006", "Čist kod - Robert C. Martin", "Priručnik o pisanju čitljivog i održivog koda. Obavezna literatura za svakog programera.", 3490m, "CAT-002"),
                Create("PROD-007", "ASP.NET Core u praksi", "Praktičan vodič kroz razvoj web aplikacija u ASP.NET Core MVC, sa Entity Framework Core primerima.", 2990m, "CAT-002"),
                Create("PROD-008", "Derviš i smrt - Meša Selimović", "Jedan od najznačajnijih romana jugoslovenske književnosti. Meki povez, 432 strane.", 1190m, "CAT-002"),

                Create("PROD-009", "Pamučna majica Basic", "Majica od 100% organskog pamuka, kratkih rukava. Dostupna u veličinama S-XXL.", 1490m, "CAT-003"),
                Create("PROD-010", "Farmerke Slim Fit", "Muške farmerke uskog kroja od elastičnog denima, tamnoplave boje.", 4990m, "CAT-003"),
                Create("PROD-011", "Patike za trčanje Runner Pro", "Lagane patike sa amortizujućim đonom i mrežastim gornjištem za bolju ventilaciju.", 9990m, "CAT-003"),
                Create("PROD-012", "Zimska jakna Nordic", "Vodootporna jakna sa kapuljačom i toplom postavom, pogodna za temperature do -15 °C.", 12990m, "CAT-003"),

                Create("PROD-013", "Aparat za espresso Barista", "Aparat za espresso sa pritiskom od 15 bara i dodatkom za penjenje mleka.", 18990m, "CAT-004"),
                Create("PROD-014", "Set tiganja 3 u 1", "Set od tri tiganja (20, 24 i 28 cm) sa granitnim premazom protiv lepljenja.", 6490m, "CAT-004"),
                Create("PROD-015", "Blender Smoothie 1000 W", "Snažan blender sa staklenom posudom od 1.5 l i pet brzina rada.", 7990m, "CAT-004"),
                Create("PROD-016", "Stona LED lampa", "Stona lampa sa podesivom jačinom i temperaturom svetla i USB priključkom za punjenje.", 3290m, "CAT-004"),

                Create("PROD-017", "Fudbalska lopta Match", "Lopta veličine 5 za trening i utakmice, šivena ručno, za sve vrste podloga.", 2490m, "CAT-005"),
                Create("PROD-018", "Podloga za jogu", "Neklizajuća podloga debljine 6 mm sa trakom za nošenje.", 2990m, "CAT-005"),
                Create("PROD-019", "Set bučica 2 x 5 kg", "Par bučica sa neoprenskom oblogom i ergonomskim drškama.", 4490m, "CAT-005"),
                Create("PROD-020", "Mountain bike 29\"", "Brdski bicikl sa aluminijumskim ramom, 21 brzinom i hidrauličnim disk kočnicama.", 54990m, "CAT-005")
            );

            await context.SaveChangesAsync();
        }
    }
}
