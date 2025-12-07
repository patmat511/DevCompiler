namespace DevCompiler.Application.DTOs;

public record CompileRequest
{
    public string Code { get; init; } = string.Empty;
}