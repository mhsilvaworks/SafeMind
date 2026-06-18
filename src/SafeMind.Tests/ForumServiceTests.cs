using System;
using System.Threading.Tasks;
using Xunit;
using SafeMind.Application.Services;
using SafeMind.Domain;
using SafeMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SafeMind.Tests
{
    public class ForumServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task ValidarAcesso_IdadeRestrita_DeveLancarAcessoNegado()
        {
            var context = GetInMemoryDbContext();
            var forumService = new ForumService(context);

            var user = new UsuarioNeurodivergente { Id = Guid.NewGuid(), BirthDate = DateTime.UtcNow.AddYears(-15) };
            var forum = new Forum { Id = Guid.NewGuid(), MinAge = 18, RequiresVerifiedStatus = false };
            
            context.Users.Add(user);
            context.Forums.Add(forum);
            await context.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => forumService.ValidarAcessoAoForumAsync(user.Id, forum.Id));
            Assert.Contains("Acesso negado. A idade mínima", exception.Message);
        }

        [Fact]
        public async Task ValidarAcesso_LaudoInautentico_DeveLancarAcessoNegado()
        {
            var context = GetInMemoryDbContext();
            var forumService = new ForumService(context);

            var user = new UsuarioNeurodivergente { Id = Guid.NewGuid(), BirthDate = DateTime.UtcNow.AddYears(-20), ValidationStatus = StatusLaudo.Rejeitado };
            var forum = new Forum { Id = Guid.NewGuid(), MinAge = 18, RequiresVerifiedStatus = true };
            
            context.Users.Add(user);
            context.Forums.Add(forum);
            await context.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => forumService.ValidarAcessoAoForumAsync(user.Id, forum.Id));
            Assert.Contains("Este fórum requer uma conta com laudo verificado", exception.Message);
        }

        [Fact]
        public async Task ValidarAcesso_UsuarioAutentico_DevePassarSemErros()
        {
            var context = GetInMemoryDbContext();
            var forumService = new ForumService(context);

            var user = new UsuarioNeurodivergente { Id = Guid.NewGuid(), BirthDate = DateTime.UtcNow.AddYears(-25), ValidationStatus = StatusLaudo.Verificado };
            var forum = new Forum { Id = Guid.NewGuid(), MinAge = 18, RequiresVerifiedStatus = true };
            
            context.Users.Add(user);
            context.Forums.Add(forum);
            await context.SaveChangesAsync();

            var exception = await Record.ExceptionAsync(() => forumService.ValidarAcessoAoForumAsync(user.Id, forum.Id));
            Assert.Null(exception);
        }

        [Fact]
        public void Post_CriarComTriggerWarning_DeveManterFlagAtiva()
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Content = "Conteúdo sensível",
                HasTriggerWarning = true
            };

            Assert.True(post.HasTriggerWarning);
        }
    }
}
