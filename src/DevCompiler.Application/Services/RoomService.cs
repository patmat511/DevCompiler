using DevCompiler.Application.Interfaces;
using DevCompiler.Application.DTOs;
using DevCompiler.Domain.Interfaces;
using DevCompiler.Domain.Entities;

namespace DevCompiler.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<RoomDto> CreateRoomAsync(CreteRoomRequest request, string userId, string nickname, CancellationToken cancellationToken = default)
    {
        var room = Room.Create(request.Name, userId, nickname);
        await _roomRepository.AddAsync(room, cancellationToken);

        return MapToDto(room);
    }

    public async Task<IEnumerable<RoomDto>> GetActiveRoomsAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _roomRepository.GetActiveRoomsAsync(cancellationToken);
        return rooms.Select(MapToDto);
    }

    public async Task<RoomDto?> GetRoomByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
       var room = await _roomRepository.GetByIdAsync(id, cancellationToken);
       return room is null ? null : MapToDto(room);
    }

    public async Task UpdateRoomCodeAsync(Guid roomId, string code, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            throw new InvalidOperationException($"Room with ID {roomId} not found");
        }

        room.UpdateCode(code);
        await _roomRepository.UpdateAsync(room, cancellationToken);
    }

    private static RoomDto MapToDto(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Code = room.Code,
            CreatedByUserId = room.CreatedByUserId,
            CreatedByNickname = room.CreatedByNickname,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt,
            IsActive = room.IsActive,
            ParticipantCount = room.Participants.Count
        };
    }
}