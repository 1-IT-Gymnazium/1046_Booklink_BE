using BooklinkBE.Data.Models;

namespace BooklinkBE.API.Services.Interfaces
{
    public interface IBookshelfService
    {
        Task<IEnumerable<Bookshelf>> GetAllAsync();
        Task<Bookshelf?> GetByIdAsync(Guid id);
        Task<Bookshelf> CreateAsync(CreateBookshelfModel model);
        Task<bool> UpdateAsync(UpdateBookshelfModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}