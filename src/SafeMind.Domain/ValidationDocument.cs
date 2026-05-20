using System;

namespace SafeMind.Domain
{
    public class ValidationDocument
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public TipoDocumento DocumentType { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
    }
}