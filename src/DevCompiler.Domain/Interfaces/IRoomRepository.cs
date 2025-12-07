using DevCompiler.Domain.Entities;

namespace DevCompiler.Domain.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>> GetActiveRoomsAsync(CancellationToken cancellationToken = default);
    Task<Room> AddSync(Room room, CancellationToken cancellationToken = default);
    Task UpdateAsync(Room room, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}