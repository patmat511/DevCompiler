using DevCompiler.Application.DTOs;
using DevCompiler.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace DevCompiler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class  AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("join")]
    public IActionResult Join([FromBody] JoinRequest request)
    {
        if (string.IsNullOrEmpty(request.Nickname))
            return BadRequest(new { error = "Nickname is required" });

        if (request.Nickname.Length < 3 || request.Nickname.Length > 20)
            return BadRequest(new { error = "Nickname must be between 3 and 20 characters" });

        var response = _authService.GenerateToken(request.Nickname);

        return Ok(response);
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] ValidatedTokenRequest request)
    {
        var nickname = _authService.ValidateToken(request.Token);

        if(nickname is null)
            return Unauthorized(new {error = "Invalid token" });

        return Ok(new { nickname });
    }
}

public record ValidatedTokenRequest(string Token);