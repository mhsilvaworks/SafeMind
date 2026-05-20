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

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                // Chama o nosso serviço SOLID e devolve a mensagem de sucesso!
                var result = _authService.Register(dto);
                return Ok(new { mensagem = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}