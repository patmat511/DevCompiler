namespace DevCompiler.Domain.Entities;

public class  Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = "// Start Coding here...";
    public string CreatedByUserId { get; private set; } = string.Empty;
    public string CreatedByNickname { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }
    
    private readonly List<string> _participants = new();
    public IReadOnlyCollection<string> Participants => _participants.AsReadOnly();

    private Room()
    {

    }

    public static Room Create(string name, string createdByUserId, string createdByNickname)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Room name cannot be empty.", nameof(name));
        }

        if (name.Length > 50)
        {
            throw new ArgumentException("Room name cannot exceed 50 characters.", nameof(name));
        }

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = "// Start Coding here...",
            CreatedByUserId = createdByUserId,
            CreatedByNickname = createdByNickname,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void AddParticipant(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (!_participants.Contains(userId))
        {
            _participants.Add(userId);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveParticipant(string userId)
    {
        _participants.Remove(userId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCode(string newCode)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot update code in an inactive room.");
        }

        if (string.IsNullOrWhiteSpace(newCode))
        {
            throw new ArgumentException("Code cannot be empty.", nameof(newCode));
        }

        Code= newCode;
        UpdatedAt = DateTime.UtcNow;
    }



}