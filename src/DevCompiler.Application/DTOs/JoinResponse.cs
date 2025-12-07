namespace DevCompiler.Application.DTOs;

public record JoinResponse
{
    public string Token { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Nickname { get; init; } = string.Empty;
}