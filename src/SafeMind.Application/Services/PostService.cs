using SafeMind.Domain;
using SafeMind.Application.Interfaces;
using SafeMind.Application.DTOs;
using SafeMind.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class PostService : IPostService
{
    private readonly AppDbContext _context;

    public PostService(AppDbContext context) => _context = context;

    public async Task<Post> CreateAsync(CreatePostDto dto, Guid userId)
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Content = dto.Content,
            ForumId = dto.ForumId,
            UserId = userId,
            HasTriggerWarning = dto.HasTriggerWarning
        };
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> GetByIdAsync(Guid id) =>
        await _context.Posts.FindAsync(id);

    public async Task<IEnumerable<Post>> GetByForumAsync(Guid forumId) =>
        await _context.Posts.Where(p => p.ForumId == forumId).ToListAsync();

    public async Task UpdateAsync(Guid postId, UpdatePostDto dto, Guid requesterId)
    {
        var post = await _context.Posts.FindAsync(postId)
            ?? throw new KeyNotFoundException("Post não encontrado.");

        if (post.UserId != requesterId)
            throw new UnauthorizedAccessException("Apenas o autor pode editar este post.");

        post.Content = dto.Content;
        post.HasTriggerWarning = dto.HasTriggerWarning;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId, Guid requesterId)
    {
        var post = await _context.Posts.FindAsync(postId)
            ?? throw new KeyNotFoundException("Post não encontrado.");

        // Dono do fórum também pode deletar posts dentro do seu fórum
        var forum = await _context.Forums.FindAsync(post.ForumId);

        if (post.UserId != requesterId && forum?.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Sem permissão para excluir este post.");

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }
}