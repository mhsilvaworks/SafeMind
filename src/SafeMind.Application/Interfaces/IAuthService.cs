using SafeMind.Application.DTOs;

namespace SafeMind.Application.Interfaces
{
    public interface IAuthService
    {
        // O contrato diz: "Quem usar este serviço tem de conseguir registar um utilizador"
        string Register(RegisterUserDto dto);
    }
}