using SafeMind.Application.DTOs;
using SafeMind.Application.Interfaces;
using SafeMind.Domain;
using SafeMind.Infrastructure.Data;
using System;
using System.Linq;
using BCrypt.Net;
// === NOVOS USINGS PARA O JWT ===
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SafeMind.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config; // A nossa ponte para o appsettings.json

        // Agora o serviço recebe a Base de Dados E as Configurações
        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ==========================================================
        // 1. MÉTODO DE REGISTO (Intacto)
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
            newUser.BirthDate = dto.BirthDate.ToUniversalTime();
            newUser.AccountType = dto.AccountType;

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return $"Sucesso! O utilizador {newUser.Nome} do tipo {newUser.AccountType} foi criado!";
        }

        // ==========================================================
        // 2. O MÉTODO DE LOGIN DEFINITIVO (KAN-4 CONCLUÍDA)
        // ==========================================================
        public string Login(LoginUserDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
            {
                throw new Exception("Utilizador ou palavra-passe incorretos.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new Exception("Utilizador ou palavra-passe incorretos.");
            }

            // === A MÁGICA DA CRIAÇÃO DO TOKEN JWT ===

            // 1. Vai buscar a chave secreta ao cofre
            var jwtKey = _config["Jwt:Key"]!;
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 2. Criar as Claims (O que o crachá diz sobre o utilizador)
            // Aqui estamos a injetar o Id e o Tipo de Conta como pede o KAN-4!
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("TipoConta", user.AccountType.ToString())
            };

            // 3. Montar a estrutura do Token (Validade de 2 horas)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = credentials
            };

            // 4. Fabricar e devolver a string final
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}