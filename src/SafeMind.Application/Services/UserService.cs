
using SafeMind.Domain;
using SafeMind.Application.Interfaces;
using SafeMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context) => _context = context;

    public async Task SetLowSpoonModeAsync(Guid userId, bool isLowSpoonMode)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (user is not UsuarioNeurodivergente neuro)
            throw new UnauthorizedAccessException("Modo Baixa Bateria Social é exclusivo para perfis Neurodivergentes.");

        neuro.IsLowSpoonMode = isLowSpoonMode;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Forum>> GetFilteredForumsAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is UsuarioNeurodivergente neuro && neuro.IsLowSpoonMode)
            return Enumerable.Empty<Forum>();

        return await _context.Forums.ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetFilteredPostsByForumAsync(Guid userId, Guid forumId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is UsuarioNeurodivergente neuro && neuro.IsLowSpoonMode)
            return Enumerable.Empty<Post>();

        return await _context.Posts
            .Where(p => p.ForumId == forumId)
            .ToListAsync();
    }
}