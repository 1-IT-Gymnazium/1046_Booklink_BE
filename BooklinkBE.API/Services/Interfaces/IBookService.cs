using BooklinkBE.Data.Models;

namespace BooklinkBE.API.Services.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetUserBooksAsync(Guid userId);
    Task<IEnumerable<Book>> GetBooksByBookshelfIdAsync(Guid id);
    Task<Book> CreateBookAsync(CreateBookRequest request);
    Task<Book> UpdateBookAsync(Guid id, UpdateBookRequest request);
    Task DeleteBookAsync(Guid id);
}