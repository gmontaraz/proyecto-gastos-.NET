using FinTrack.Api.DTOs;
using FinTrack.Api.Services;
using Microsoft.Extensions.Configuration;

namespace FinTrack.Tests
{
    public class AuthServiceTests
    {
        private AuthService CreateService()
        {
            var settings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "clave_para_los_tests_1234567890_abcdefgh" },
                { "Jwt:Issuer", "test" },
                { "Jwt:Audience", "test" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

            return new AuthService(TestHelper.CreateContext(), config);
        }

        [Fact]
        public async Task Register_ReturnsToken()
        {
            var service = CreateService();

            var result = await service.Register(new RegisterDto { Name = "Pepe", Email = "pepe@test.com", Password = "123456" });

            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }

        [Fact]
        public async Task Register_WithSameEmail_ReturnsNull()
        {
            var service = CreateService();
            await service.Register(new RegisterDto { Name = "Pepe", Email = "pepe@test.com", Password = "123456" });

            var result = await service.Register(new RegisterDto { Name = "Otro", Email = "PEPE@test.com", Password = "123456" });

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsNull()
        {
            var service = CreateService();
            await service.Register(new RegisterDto { Name = "Pepe", Email = "pepe@test.com", Password = "123456" });

            var result = await service.Login(new LoginDto { Email = "pepe@test.com", Password = "otra" });

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_Ok()
        {
            var service = CreateService();
            await service.Register(new RegisterDto { Name = "Pepe", Email = "pepe@test.com", Password = "123456" });

            var result = await service.Login(new LoginDto { Email = "pepe@test.com", Password = "123456" });

            Assert.NotNull(result);
            Assert.Equal("Pepe", result.Name);
        }
    }
}
