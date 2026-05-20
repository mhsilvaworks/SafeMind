using SafeMind.Application.DTOs;
using SafeMind.Application.Interfaces;
using SafeMind.Domain;
using SafeMind.Infrastructure.Data;
using System;
using System.Linq; // Muito importante para a procura na base de dados!
using BCrypt.Net; 

namespace SafeMind.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. MÉTODO DE REGISTO (O que já estava a funcionar 100%)
        // ==========================================================
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

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return $"Sucesso! O utilizador {newUser.Nome} do tipo {newUser.AccountType} foi criado!";
        }

        // ==========================================================
        // 2. O NOVO MÉTODO DE LOGIN (Para a task KAN-4)
        // ==========================================================
        public string Login(LoginUserDto dto)
        {
            // 1. Procura o utilizador pelo Email na base de dados
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
            {
                // Por segurança, nunca dizemos se o que falhou foi o email ou a senha
                throw new Exception("Utilizador ou palavra-passe incorretos.");
            }

            // 2. A Mágica do BCrypt: Ele verifica se a senha digitada bate com o Hash guardado
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new Exception("Utilizador ou palavra-passe incorretos.");
            }

            // 3. Retorno temporário até criarmos o Token JWT
            return "Login efetuado com sucesso! Preparado para receber o JWT.";
        }
    }
}