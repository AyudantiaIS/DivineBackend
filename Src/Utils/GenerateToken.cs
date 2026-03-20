using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Src.Models;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Src.Utils;

public static class GenerateToken
{
    public static string CreateToken(User user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:key"]!) 
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.Name) 
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:issuer"],
            audience: config["Jwt:audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}