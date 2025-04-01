using BooklinkBE.Data.Models;

namespace BooklinkBE.API.Services.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<Room>> GetRoomsByUserIdAsync(Guid userId);
    Task<IEnumerable<Room>> GetRoomsAsync(Guid householdId);
    Task<Room?> GetRoomByIdAsync(Guid id);
    Task<Room> CreateRoomAsync(Room room);
    Task<bool> UpdateRoomAsync(Guid id, UpdateRoomRequest request);
    Task<bool> DeleteRoomAsync(Guid id);
}