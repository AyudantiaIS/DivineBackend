using Backend.Src.Dtos.Users;
using Backend.Src.Services.Interfaces;
using Backend.Src.Utils;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Services;

public class AuthServices(AppDbContext context, IConfiguration configuration) : IAuthService
{
    private readonly AppDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    public Task<LoginResponse> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u=> u.Email == loginDto.Email);

            if(user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Task.FromResult(new LoginResponse
                {
                    Success = false,
                    Message = "Correo electrónico o contraseña incorrectos",
                    Data = null
                });
            }
            if (!user.Status)
            {
                return Task.FromResult(new LoginResponse
                {
                    Success = false,
                    Message = "Usuario inactivo, por favor contacte al administrador",
                    Data = null
                });
            }
            return Task.FromResult(new LoginResponse
            {
                Success = true,
                Message = "Login exitoso",
                Data = new DataResponseLogin
                {
                    Token = GenerateToken.CreateToken(user, _configuration),
                    UserId = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.Name
                }
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new LoginResponse
            {
                Success = false,
                Message = $"Error durante el login: {ex.Message}",
                Data = null
            });
        }
        throw new NotImplementedException();
    }
}