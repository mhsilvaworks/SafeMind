using SafeMind.Application.DTOs;

namespace SafeMind.Application.Interfaces
{
    public interface IAuthService
    {
        string Register(RegisterUserDto dto);
        
        // A nossa nova linha para o KAN-4!
        string Login(LoginUserDto dto); 
    }
}