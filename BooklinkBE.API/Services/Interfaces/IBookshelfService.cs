using BooklinkBE.Data.Models;

namespace BooklinkBE.API.Services.Interfaces
{
    public interface IBookshelfService
    {
        Task<IEnumerable<Bookshelf>> GetBookshelvesByUserIdAsync(Guid userId);
        Task<IEnumerable<Bookshelf>> GetBookshelvesByRoomIdAsync(Guid roomId);
        Task<Bookshelf?> GetBookshelfById(Guid id);
        Task<Bookshelf> CreateBookshelfAsync(CreateBookshelfModel model);
        Task<bool> UpdateBookshelfAsync(UpdateBookshelfModel model);
        Task<bool> DeleteRoomAsync(Guid id);
    }
}