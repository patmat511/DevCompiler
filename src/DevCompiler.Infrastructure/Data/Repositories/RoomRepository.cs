using DevCompiler.Domain.Entities;
using DevCompiler.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevCompiler.Infrastructure.Data.Repositories;

public class RoomReposiotry : IRoomRepository
{
    private readonly AppDbContext _context;
    public RoomReposiotry(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return room;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Room>> GetActiveRoomsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.UpdatedAt)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        _context.Rooms.Update(room);    
        await _context.SaveChangesAsync(cancellationToken);
    }
}