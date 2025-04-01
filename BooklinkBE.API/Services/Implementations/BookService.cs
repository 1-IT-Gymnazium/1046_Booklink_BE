using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.API.Services.Implementations
{
    public class BookService(AppDbContext context) : IBookService
    {
        public async Task<IEnumerable<Book>> GetUserBooksAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID");

            return await context.Books
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Book>> GetBooksByBookshelfIdAsync(Guid bookshelfId)
        {
            return await context.Books
                .Where(b => b.BookshelfId == bookshelfId)
                .ToListAsync();
        }
        
        public async Task<Book> CreateBookAsync(CreateBookRequest request)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Title = request.Title,
                Author = request.Author,
                Genre = request.Genre,
                Isbn = request.Isbn,
                PublicationYear = request.PublicationYear,
                IsInReadingList = request.IsInReadingList,
                ColumnsFromLeft = request.ColumnsFromLeft,
                RowsFromTop = request.RowsFromTop,
                HouseholdId = request.HouseholdId,
                RoomId = request.RoomId,
                BookshelfId = request.BookshelfId,
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();
            
            return book;
        }

        public async Task<Book> UpdateBookAsync(Guid id, UpdateBookRequest request)
        {
            if (id != request.Id)
                throw new ArgumentException("Book ID mismatch.");

            var book = await context.Books.FindAsync(id);
            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            book.Title = request.Title;
            book.Author = request.Author;
            book.Genre = request.Genre;
            book.Isbn = request.Isbn;
            book.PublicationYear = request.PublicationYear;
            book.IsInReadingList = request.IsInReadingList;
            book.ColumnsFromLeft = request.ColumnsFromLeft;
            book.RowsFromTop = request.RowsFromTop;
            book.HouseholdId = request.HouseholdId;
            book.RoomId = request.RoomId;
            book.BookshelfId = request.BookshelfId;

            context.Books.Update(book);
            await context.SaveChangesAsync();

            return book;
        }
        
        public async Task DeleteBookAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid book ID");

            var book = await context.Books.FindAsync(id);
            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            context.Books.Remove(book);
            await context.SaveChangesAsync();
        }
    }
}