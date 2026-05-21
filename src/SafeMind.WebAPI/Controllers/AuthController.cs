using Microsoft.AspNetCore.Mvc;
using SafeMind.Application.DTOs;
using SafeMind.Application.Interfaces;
using System;

namespace SafeMind.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
            
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ==========================================
        // 1. ROTA DE REGISTO (A que já tínhamos)
        // ==========================================
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var result = _authService.Register(dto);
                return Ok(new { mensagem = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // ==========================================
        // 2. NOVA ROTA DE LOGIN (Para o KAN-4)
        // ==========================================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserDto dto)
        {
            try
            {
                // Chama o serviço para validar as credenciais (Email e Palavra-passe)
                var result = _authService.Login(dto);
                
                // Se der certo, devolve o 200 OK com a mensagem temporária
                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                // Se a palavra-passe estiver errada, devolve 401 Unauthorized (padrão ouro de segurança)
                return Unauthorized(new { erro = ex.Message });
            }
        }
    }
}