public class CreatePostDto
{
    public string Content { get; set; } = string.Empty;
    public Guid ForumId { get; set; }
    public bool HasTriggerWarning { get; set; }
}