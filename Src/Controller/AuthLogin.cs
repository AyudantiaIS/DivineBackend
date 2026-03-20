
using System.Net;
using Backend.Src.Dtos.Users;
using Backend.Src.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (!result.Success || result.Data == null)
        {
            return BadRequest(result);
        }

        Response.Cookies.Append("token", result.Data.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        }); 
        
        return Ok(result);

    }
}