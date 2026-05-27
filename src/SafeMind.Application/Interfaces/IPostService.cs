using SafeMind.Domain;
using SafeMind.Application.DTOs;

public interface IPostService
{
    Task<Post> CreateAsync(CreatePostDto dto, Guid userId);
    Task<Post?> GetByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetByForumAsync(Guid forumId);
    Task UpdateAsync(Guid postId, UpdatePostDto dto, Guid requesterId);
    Task DeleteAsync(Guid postId, Guid requesterId);
}