using FinTrack.Api.Models;

namespace FinTrack.Api.Data
{
    public static class DefaultCategories
    {
        // Categorias que se crean al registrarse un usuario
        public static List<Category> Get()
        {
            return new List<Category>
            {
                new Category { Name = "Nómina", Type = TransactionType.Income },
                new Category { Name = "Otros ingresos", Type = TransactionType.Income },
                new Category { Name = "Comida", Type = TransactionType.Expense },
                new Category { Name = "Casa", Type = TransactionType.Expense },
                new Category { Name = "Transporte", Type = TransactionType.Expense },
                new Category { Name = "Ocio", Type = TransactionType.Expense },
                new Category { Name = "Otros", Type = TransactionType.Expense }
            };
        }
    }
}
