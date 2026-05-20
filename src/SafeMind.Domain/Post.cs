using System;

namespace SafeMind.Domain
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid ForumId { get; set; }
        public bool HasTriggerWarning { get; set; }
    }
}