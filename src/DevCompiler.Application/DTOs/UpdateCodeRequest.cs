namespace DevCompiler.Application.Entities;

public record UpdateCodeRequest
{
    public string Code { get; init; } = string.Empty;
}