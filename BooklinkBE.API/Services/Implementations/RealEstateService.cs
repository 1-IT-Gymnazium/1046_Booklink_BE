using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.API.Services.Implementations;

public class RealEstateService : IRealEstateService
{
    private readonly AppDbContext _dbContext;

    public RealEstateService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateRealEstate(CreateRealEstateRequest request)
    {
        var newLocation = new RealEstate
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Name = request.Name,
        };

        _dbContext.RealEstates.Add(newLocation);
        await _dbContext.SaveChangesAsync();

        return newLocation.Id;
    }

    public async Task<RealEstate?> GetRealEstatesById(Guid id)
    {
        return await _dbContext.RealEstates.FindAsync(id);
    }

    public async Task<bool> EditRealEstate(UpdateRealEstateRequest request)
    {
        var location = await _dbContext.RealEstates.FirstOrDefaultAsync(loc => loc.Id == request.Id);
        if (location == null)
            return false;

        location.Name = request.Name;

        _dbContext.RealEstates.Update(location);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task DeleteRealEstate(Guid id)
    {
        var location = await _dbContext.RealEstates
            .Include(loc => loc.Books)
            .FirstOrDefaultAsync(loc => loc.Id == id);

        if (location == null)
            throw new ArgumentException("Location not found.");

        if (location.Books.Any())
            throw new InvalidOperationException("Cannot delete location with associated books.");

        _dbContext.RealEstates.Remove(location);
        await _dbContext.SaveChangesAsync();
    }
}