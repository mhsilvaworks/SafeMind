using System;
using Xunit;
using Moq;
using SafeMind.Application.Services;
using SafeMind.Application.DTOs;
using SafeMind.Domain;
using SafeMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SafeMind.Tests
{
    public class AuthServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void Login_SenhaDivergente_DeveLancarExcecao()
        {
            var context = GetInMemoryDbContext();
            var mockConfig = new Mock<IConfiguration>();
            var authService = new AuthService(context, mockConfig.Object);

            var user = new UsuarioNeurodivergente
            {
                Id = Guid.NewGuid(),
                Email = "teste@safemind.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SenhaCorreta123"),
                AccountType = TipoConta.Neurodivergente
            };
            context.Users.Add(user);
            context.SaveChanges();

            var loginDto = new LoginUserDto { Email = "teste@safemind.com", Password = "SenhaIncorreta" };

            var exception = Assert.Throws<Exception>(() => authService.Login(loginDto));
            Assert.Equal("Utilizador ou palavra-passe incorretos.", exception.Message);
        }
    }
}
