using System;
using SafeMind.Domain;

namespace SafeMind.Application.DTOs
{
    public class RegisterUserDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 
        public DateTime BirthDate { get; set; }
        public TipoConta AccountType { get; set; }

        // Campos para os Documentos e Validações
        public string CpfCnpj { get; set; } = string.Empty;
        public string UrlLaudo { get; set; } = string.Empty; 
        public string UrlCertificadoProfissional { get; set; } = string.Empty;
    }
}