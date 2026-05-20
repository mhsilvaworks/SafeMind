using System;

namespace SafeMind.Domain
{
    public class Forum
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public TipoForum ForumType { get; set; } 
        public int MinAge { get; set; }
        public bool RequiresVerifiedStatus { get; set; }
    }
}