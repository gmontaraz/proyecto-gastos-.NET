using FinTrack.Api.DTOs;
using FinTrack.Api.Models;
using FinTrack.Api.Services;

namespace FinTrack.Tests
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task Create_TypeComesFromCategory()
        {
            var context = TestHelper.CreateContext();
            var user = TestHelper.CreateUser(context, "a@test.com");
            var service = new TransactionService(context);
            var nomina = user.Categories.First(c => c.Name == "Nómina");

            var result = await service.Create(user.Id, new CreateTransactionDto
            {
                Description = "Sueldo",
                Amount = 1500,
                Date = new DateTime(2026, 5, 1),
                CategoryId = nomina.Id
            });

            Assert.NotNull(result);
            Assert.Equal(TransactionType.Income, result.Type);
        }

        [Fact]
        public async Task Create_WithCategoryOfAnotherUser_ReturnsNull()
        {
            var context = TestHelper.CreateContext();
            var user1 = TestHelper.CreateUser(context, "a@test.com");
            var user2 = TestHelper.CreateUser(context, "b@test.com");
            var service = new TransactionService(context);

            var result = await service.Create(user1.Id, new CreateTransactionDto
            {
                Description = "Prueba",
                Amount = 10,
                Date = DateTime.Today,
                CategoryId = user2.Categories.First().Id
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByMonth_OnlyReturnsThatMonth()
        {
            var context = TestHelper.CreateContext();
            var user = TestHelper.CreateUser(context, "a@test.com");
            var service = new TransactionService(context);
            var comida = user.Categories.First(c => c.Name == "Comida");

            await service.Create(user.Id, new CreateTransactionDto { Description = "abril", Amount = 10, Date = new DateTime(2026, 4, 30), CategoryId = comida.Id });
            await service.Create(user.Id, new CreateTransactionDto { Description = "mayo", Amount = 10, Date = new DateTime(2026, 5, 2), CategoryId = comida.Id });
            await service.Create(user.Id, new CreateTransactionDto { Description = "mayo 2", Amount = 10, Date = new DateTime(2026, 5, 20), CategoryId = comida.Id });

            var result = await service.GetByMonth(user.Id, 2026, 5);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Delete_TransactionOfAnotherUser_ReturnsFalse()
        {
            var context = TestHelper.CreateContext();
            var user1 = TestHelper.CreateUser(context, "a@test.com");
            var user2 = TestHelper.CreateUser(context, "b@test.com");
            var service = new TransactionService(context);

            var created = await service.Create(user1.Id, new CreateTransactionDto
            {
                Description = "Mio",
                Amount = 20,
                Date = DateTime.Today,
                CategoryId = user1.Categories.First().Id
            });

            var deleted = await service.Delete(user2.Id, created!.Id);

            Assert.False(deleted);
        }

        [Fact]
        public async Task GetSummary_CalculatesTotals()
        {
            var context = TestHelper.CreateContext();
            var user = TestHelper.CreateUser(context, "a@test.com");
            var service = new TransactionService(context);
            var nomina = user.Categories.First(c => c.Name == "Nómina");
            var comida = user.Categories.First(c => c.Name == "Comida");
            var ocio = user.Categories.First(c => c.Name == "Ocio");

            await service.Create(user.Id, new CreateTransactionDto { Description = "Sueldo", Amount = 1000, Date = new DateTime(2026, 5, 1), CategoryId = nomina.Id });
            await service.Create(user.Id, new CreateTransactionDto { Description = "Super", Amount = 100, Date = new DateTime(2026, 5, 3), CategoryId = comida.Id });
            await service.Create(user.Id, new CreateTransactionDto { Description = "Super", Amount = 50, Date = new DateTime(2026, 5, 10), CategoryId = comida.Id });
            await service.Create(user.Id, new CreateTransactionDto { Description = "Cine", Amount = 20, Date = new DateTime(2026, 5, 12), CategoryId = ocio.Id });

            var summary = await service.GetSummary(user.Id, 2026, 5);

            Assert.Equal(1000, summary.TotalIncome);
            Assert.Equal(170, summary.TotalExpenses);
            Assert.Equal(830, summary.Balance);
            Assert.Equal(2, summary.ExpensesByCategory.Count);
            Assert.Equal(150, summary.ExpensesByCategory.First(x => x.Category == "Comida").Total);
        }
    }
}
