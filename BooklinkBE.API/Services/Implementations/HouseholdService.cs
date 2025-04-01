using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.API.Services.Implementations;

public class HouseholdService(AppDbContext dbContext) : IHouseholdService
{
    public async Task<Household?> GetHouseholdByIdAsync(Guid id)
    {
        return await dbContext.Households.FindAsync(id);
    }
    
    public async Task<IEnumerable<Household>> GetHouseholdsByUserIdAsync(Guid userId)
    {
        return await dbContext.Households
            .Where(e => e.UserId == userId)
            .Select(e => new Household { Id = e.Id, Name = e.Name })
            .ToListAsync();
    }

    public async Task<Guid> CreateHouseholdAsync(CreateHouseholdRequest request)
    {
        var newHousehold = new Household
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Name = request.Name,
        };

        dbContext.Households.Add(newHousehold);
        await dbContext.SaveChangesAsync();
        
        return newHousehold.Id;
    }
    
    public async Task<bool> UpdateHouseholdAsync(UpdateHouseholdRequest request)
    {
        var location = await dbContext.Households.FirstOrDefaultAsync(loc => loc.Id == request.Id);
        if (location == null)
            return false;

        location.Name = request.Name;

        dbContext.Households.Update(location);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task DeleteHouseholdAsync(Guid id)
    {
        var location = await dbContext.Households
            .Include(loc => loc.Books)
            .FirstOrDefaultAsync(loc => loc.Id == id);

        if (location == null)
            throw new ArgumentException("Location not found.");

        dbContext.Households.Remove(location);
        await dbContext.SaveChangesAsync();
    }
}