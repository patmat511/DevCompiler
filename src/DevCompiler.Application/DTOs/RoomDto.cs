namespace DevCompiler.Application.DTOs;

public record RoomDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string CreatedByUserId { get; init; } = string.Empty;
    public string CreatedByNickname { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsActive { get; init; }
    public int ParticipantCount { get; init; }
}