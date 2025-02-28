using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.API.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Book>> GetUserBooksAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID");

            return await _context.Books
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
        
        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid book ID");

            var book = await _context.Books.FindAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            return book;
        }
        
        public async Task<Book> CreateBookAsync(Guid userId, CreateBookRequest request)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID");

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Author))
                throw new ArgumentException("Book title and author are required.");

            var book = new Book
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Author = request.Author,
                Genre = request.Genre,
                ISBN = request.ISBN,
                PublicationYear = request.PublicationYear
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateBookAsync(Guid id, UpdateBookRequest request)
        {
            if (id != request.Id)
                throw new ArgumentException("Book ID mismatch.");

            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            book.Title = request.Title;
            book.Author = request.Author;
            book.Genre = request.Genre;
            book.ISBN = request.ISBN;
            book.PublicationYear = request.PublicationYear;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return book;
        }
        
        public async Task DeleteBookAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid book ID");

            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}