using SafeMind.Domain;

public class CreateForumDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TipoForum ForumType { get; set; }
    public int MinAge { get; set; }
    public bool RequiresVerifiedStatus { get; set; }
}