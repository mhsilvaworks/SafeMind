using System;

namespace SafeMind.Domain
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool HasTriggerWarning { get; set; }

        // Chaves Estrangeiras e Navegação
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public Guid ForumId { get; set; }
        public virtual Forum Forum { get; set; } = null!;
    }
}