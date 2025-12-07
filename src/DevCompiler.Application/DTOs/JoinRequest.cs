namespace DevCompiler.Application.DTOs;

public record JoinRequest
{
    public string Nickname { get; init; } = string.Empty;
}