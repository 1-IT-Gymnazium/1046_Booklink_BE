using BooklinkBE.Data.Models;

namespace BooklinkBE.API.Services.Interfaces;

public interface IHouseholdService
{
    Task<IEnumerable<Household>> GetHouseholdsByUserIdAsync(Guid userId);
    Task<Guid> CreateHouseholdAsync(CreateHouseholdRequest request);
    Task<Household?> GetHouseholdByIdAsync(Guid id);
    Task<bool> UpdateHouseholdAsync(UpdateHouseholdRequest request);
    Task DeleteHouseholdAsync(Guid id);
}