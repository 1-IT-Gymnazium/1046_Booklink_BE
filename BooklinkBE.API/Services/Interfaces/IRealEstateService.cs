using BooklinkBE.Data.Models;

namespace BooklinkBE.API.Services.Interfaces;

public interface IRealEstateService
{
    Task<Guid> CreateRealEstate(CreateRealEstateRequest request);
    Task<RealEstate?> GetRealEstatesById(Guid id);
    Task<bool> EditRealEstate(UpdateRealEstateRequest request);
    Task DeleteRealEstate(Guid id);
}