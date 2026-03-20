using Backend.Src.Dtos.Users;
namespace Backend.Src.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginDto loginDto);
}