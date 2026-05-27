using SafeMind.Domain;
using SafeMind.Application.Interfaces;
using SafeMind.Application.DTOs;
using SafeMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
public class ForumService : IForumService
{
    private readonly AppDbContext _context;

    public ForumService(AppDbContext context) => _context = context;

    public async Task<Forum> CreateAsync(CreateForumDto dto, Guid ownerId)
    {
        var forum = new Forum
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            OwnerId = ownerId,
            ForumType = dto.ForumType,
            MinAge = dto.MinAge,
            RequiresVerifiedStatus = dto.RequiresVerifiedStatus
        };
        _context.Forums.Add(forum);
        await _context.SaveChangesAsync();
        return forum;
    }

    public async Task<Forum?> GetByIdAsync(Guid id) =>
        await _context.Forums.FindAsync(id);

    public async Task<IEnumerable<Forum>> GetAllAsync() =>
        await _context.Forums.ToListAsync();

    public async Task UpdateAsync(Guid forumId, UpdateForumDto dto, Guid requesterId)
    {
        var forum = await _context.Forums.FindAsync(forumId)
            ?? throw new KeyNotFoundException("Fórum não encontrado.");

        if (forum.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Apenas o criador do fórum pode editá-lo.");

        forum.Title = dto.Title;
        forum.Description = dto.Description;
        forum.MinAge = dto.MinAge;
        forum.RequiresVerifiedStatus = dto.RequiresVerifiedStatus;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid forumId, Guid requesterId)
    {
        var forum = await _context.Forums.FindAsync(forumId)
            ?? throw new KeyNotFoundException("Fórum não encontrado.");

        if (forum.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Apenas o criador do fórum pode excluí-lo.");

        _context.Forums.Remove(forum);
        await _context.SaveChangesAsync();
    }
}