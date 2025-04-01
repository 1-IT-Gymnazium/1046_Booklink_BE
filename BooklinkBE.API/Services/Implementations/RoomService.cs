using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.API.Services.Implementations;

public class RoomService(AppDbContext context) : IRoomService
{
    public async Task<IEnumerable<Room>> GetRoomsByUserIdAsync(Guid userId)
    {
        return await context.Rooms.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetRoomsAsync(Guid householdId)
    {
        return await context.Rooms.Where(x => x.HouseholdId == householdId).ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(Guid id)
    {
        return await context.Rooms.Include(r => r.Household).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room> CreateRoomAsync(Room room)
    {
        var household = await context.Households.FindAsync(room.HouseholdId);
        if (household == null)
            throw new ArgumentException("Household not found.");

        context.Rooms.Add(room);
        await context.SaveChangesAsync();
        return room;
    }

    public async Task<bool> UpdateRoomAsync(Guid id, UpdateRoomRequest request)
    {
        var room = await context.Rooms.FindAsync(id);
        if (room == null)
            return false;

        var household = await context.Households.FindAsync(request.HouseholdId);
        if (household == null)
            throw new ArgumentException("Household not found.");

        room.Name = request.Name;
        room.HouseholdId = request.HouseholdId;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRoomAsync(Guid id)
    {
        var room = await context.Rooms.FindAsync(id);
        if (room == null)
            return false;

        context.Rooms.Remove(room);
        await context.SaveChangesAsync();
        return true;
    }
}