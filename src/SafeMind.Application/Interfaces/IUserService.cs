using SafeMind.Domain;

public interface IUserService
{
    Task SetLowSpoonModeAsync(Guid userId, bool isLowSpoonMode);
    Task<IEnumerable<Forum>> GetFilteredForumsAsync(Guid userId);
    Task<IEnumerable<Post>> GetFilteredPostsByForumAsync(Guid userId, Guid forumId);
}