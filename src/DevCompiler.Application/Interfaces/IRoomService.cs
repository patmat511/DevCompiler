using DevCompiler.Application.DTOs;

namespace DevCompiler.Application.Interfaces;

public interface IRoomService
{
    Task<RoomDto> CreateRoomAsync(CreteRoomRequest request, string userId, string nickname, CancellationToken cancellationToken = default);
    Task<RoomDto?> GetRoomByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomDto>> GetActiveRoomsAsync(CancellationToken cancellationToken = default);
    Task UpdateRoomCodeAsync(Guid roomId, string code, CancellationToken cancellationToken = default);
}