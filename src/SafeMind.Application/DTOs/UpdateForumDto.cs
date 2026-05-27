using SafeMind.Domain;
public class UpdateForumDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public bool RequiresVerifiedStatus { get; set; }
}