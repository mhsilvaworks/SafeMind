using System;

namespace SafeMind.Domain
{
    // Enums definidos no Diagrama de Classes   
    public enum TipoConta
    {
        Neurodivergente,
        Profissional,
        Empresa
    }

    public enum StatusLaudo
    {
        Pendente,
        Verificado,
        Rejeitado
    }

    // A classe principal do usuário
    public class User
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        
        public TipoConta AccountType { get; set; }
        public StatusLaudo ValidationStatus { get; set; }
        
        // Flag do Modo Baixa Bateria Social (RN04)
        public bool IsLowSpoonMode { get; set; }
    }
}