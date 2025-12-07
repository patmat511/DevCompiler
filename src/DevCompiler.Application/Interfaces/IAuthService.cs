using DevCompiler.Application.DTOs;

namespace DevCompiler.Application.Interfaces;

public interface IAuthService
{
    JoinResponse GenerateToken(string nickname);
    string? ValidateToken(string token);
}