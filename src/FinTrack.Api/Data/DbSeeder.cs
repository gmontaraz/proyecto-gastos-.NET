using FinTrack.Api.Models;

namespace FinTrack.Api.Data
{
    // Mete un usuario de prueba con algunos movimientos para no tener la app vacia
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            var user = new User
            {
                Name = "Demo",
                Email = "demo@fintrack.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"),
                Categories = DefaultCategories.Get()
            };

            context.Users.Add(user);
            context.SaveChanges();

            var today = DateTime.Today;
            var firstDay = new DateTime(today.Year, today.Month, 1);

            AddTransaction(context, user, "Nómina", "Nomina", 1650, firstDay);
            AddTransaction(context, user, "Casa", "Alquiler", 600, firstDay.AddDays(2));
            AddTransaction(context, user, "Comida", "Mercadona", 54.30m, firstDay.AddDays(3));
            AddTransaction(context, user, "Transporte", "Gasolina", 45, firstDay.AddDays(5));
            AddTransaction(context, user, "Ocio", "Cine", 18, firstDay.AddDays(6));
            AddTransaction(context, user, "Comida", "Lidl", 32.15m, firstDay.AddDays(8));
            AddTransaction(context, user, "Ocio", "Cena con amigos", 35, firstDay.AddDays(10));
            AddTransaction(context, user, "Otros", "Regalo cumple", 25, firstDay.AddDays(12));
            AddTransaction(context, user, "Comida", "Mercadona", 61.80m, firstDay.AddDays(14));

            context.SaveChanges();
        }

        private static void AddTransaction(AppDbContext context, User user, string categoryName, string description, decimal amount, DateTime date)
        {
            // para que no salgan movimientos en el futuro si estamos a principio de mes
            if (date > DateTime.Today)
                return;

            var category = user.Categories.First(c => c.Name == categoryName);

            context.Transactions.Add(new Transaction
            {
                Description = description,
                Amount = amount,
                Date = date,
                Type = category.Type,
                CategoryId = category.Id,
                UserId = user.Id
            });
        }
    }
}
