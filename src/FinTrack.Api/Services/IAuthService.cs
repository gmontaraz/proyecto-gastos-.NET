using FinTrack.Api.DTOs;

namespace FinTrack.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> Register(RegisterDto dto);
        Task<AuthResponseDto?> Login(LoginDto dto);
    }
}
