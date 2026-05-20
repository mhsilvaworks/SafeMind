using System;
using System.Collections.Generic;

namespace SafeMind.Domain
{
    public class Forum
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TipoForum ForumType { get; set; } 
        public int MinAge { get; set; }
        public bool RequiresVerifiedStatus { get; set; }

        // Chave Estrangeira e Propriedade de Navegação
        public Guid OwnerId { get; set; }
        public virtual User Owner { get; set; } = null!;

        // Um fórum tem muitos posts
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}