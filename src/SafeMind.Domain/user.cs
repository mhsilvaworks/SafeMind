using System;
using System.Collections.Generic;

namespace SafeMind.Domain
{

    public interface IValidavel
    {
        StatusLaudo ValidationStatus { get; set; }
    }


    public abstract class User
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public TipoConta AccountType { get; set; }

        
        public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();
        
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        public virtual ValidationDocument? ValidationDocument { get; set; }
    }

    public class Administrador : User
    {
        public string NivelAcesso { get; set; } = "Moderador"; 
        public string Departamento { get; set; } = string.Empty;
    }

    public class UsuarioNeurodivergente : User, IValidavel
    {
        public StatusLaudo ValidationStatus { get; set; } 
        public bool IsLowSpoonMode { get; set; }
        public List<string> Hiperfocos { get; set; } = new();
        public string Diagnostico { get; set; } = string.Empty;
        public string PreferenciasSensoriais { get; set; } = string.Empty;
    }

    public class Profissional : User, IValidavel
    {
        public string Cpf { get; set; } = string.Empty;
        public StatusLaudo ValidationStatus { get; set; } 
    }

    public class Empresa : User, IValidavel
    {
        public string Cnpj { get; set; } = string.Empty;
        public StatusLaudo ValidationStatus { get; set; }
    }
}