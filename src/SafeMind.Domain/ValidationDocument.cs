using System;

namespace SafeMind.Domain
{
    public class ValidationDocument
    {
        public Guid Id { get; set; }
        public TipoDocumento DocumentType { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }

        // Chave Estrangeira e Navegação (1 para Zero ou Um)
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}