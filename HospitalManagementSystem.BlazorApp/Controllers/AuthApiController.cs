using HospitalManagementSystem.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HospitalManagementSystem.BlazorApp.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthApiController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;

    public AuthApiController(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _jwtTokenService.GenerateTokenAsync(request.Email, request.Password);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(new { token });
    }
}

public record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password);
