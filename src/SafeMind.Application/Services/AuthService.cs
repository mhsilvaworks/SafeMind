using SafeMind.Application.DTOs;
using SafeMind.Application.Interfaces;
using SafeMind.Domain;
using SafeMind.Infrastructure.Data; // ADICIONADO: Para ele conhecer o banco de dados
using System;
using BCrypt.Net; 

namespace SafeMind.Application.Services
{
    public class AuthService : IAuthService
    {
        // 1. ADICIONADO: A variável que vai segurar a conexão com o banco
        private readonly AppDbContext _context;

        // 2. ADICIONADO: O Construtor que recebe o banco (Injeção de Dependência)
        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public string Register(RegisterUserDto dto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            User newUser;

            switch (dto.AccountType)
            {
                case TipoConta.Neurodivergente:
                    var nd = new UsuarioNeurodivergente
                    {
                        ValidationStatus = StatusLaudo.Pendente,
                        IsLowSpoonMode = false
                    };
                    if (!string.IsNullOrEmpty(dto.UrlLaudo))
                    {
                        nd.ValidationDocument = new ValidationDocument
                        {
                            Id = Guid.NewGuid(),
                            DocumentType = TipoDocumento.LaudoMedico,
                            FileUrl = dto.UrlLaudo,
                            DataEnvio = DateTime.UtcNow
                        };
                    }
                    newUser = nd;
                    break;

                case TipoConta.Profissional:
                    var prof = new Profissional
                    {
                        Cpf = dto.CpfCnpj,
                        ValidationStatus = StatusLaudo.Pendente
                    };
                    if (!string.IsNullOrEmpty(dto.UrlCertificadoProfissional))
                    {
                        prof.ValidationDocument = new ValidationDocument
                        {
                            Id = Guid.NewGuid(),
                            DocumentType = TipoDocumento.CRM, 
                            FileUrl = dto.UrlCertificadoProfissional,
                            DataEnvio = DateTime.UtcNow
                        };
                    }
                    newUser = prof;
                    break;

                case TipoConta.Empresa:
                    newUser = new Empresa
                    {
                        Cnpj = dto.CpfCnpj,
                        ValidationStatus = StatusLaudo.Pendente
                    };
                    break;
                
                case TipoConta.Administrador:
                    newUser = new Administrador
                    {
                        NivelAcesso = "Moderador",
                        Departamento = "Geral"
                    };
                    break;

                default:
                    throw new Exception("Tipo de conta inválido.");
            }

            newUser.Id = Guid.NewGuid();
            newUser.Nome = dto.Nome;
            newUser.Email = dto.Email;
            newUser.PasswordHash = passwordHash;
            newUser.BirthDate = dto.BirthDate;
            newUser.AccountType = dto.AccountType;

            // 3. ADICIONADO: O Grande Final!
            // Avisamos o Entity Framework que temos um dado novo...
            _context.Users.Add(newUser);
            
            // ...e mandamos salvar fisicamente no Docker!
            _context.SaveChanges();

            return $"Sucesso! O utilizador {newUser.Nome} do tipo {newUser.AccountType} foi criado!";
        }
    }
}