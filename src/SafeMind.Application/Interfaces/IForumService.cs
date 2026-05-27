using SafeMind.Domain;
using SafeMind.Application.DTOs;

public interface IForumService
{
    Task<Forum> CreateAsync(CreateForumDto dto, Guid ownerId);
    Task<Forum?> GetByIdAsync(Guid id);
    Task<IEnumerable<Forum>> GetAllAsync();
    Task UpdateAsync(Guid forumId, UpdateForumDto dto, Guid requesterId);
    Task DeleteAsync(Guid forumId, Guid requesterId);
}