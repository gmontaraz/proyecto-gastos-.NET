using FinTrack.Api.Data;
using FinTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Tests
{
    public static class TestHelper
    {
        // base de datos en memoria nueva para cada test
        public static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        public static User CreateUser(AppDbContext context, string email)
        {
            var user = new User
            {
                Name = "Test",
                Email = email,
                PasswordHash = "1234",
                Categories = DefaultCategories.Get()
            };

            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }
    }
}
