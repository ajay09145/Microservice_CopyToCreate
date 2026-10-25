using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegistrationRequestDto registerationRequestDto);
        Task<LoginResonseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}
