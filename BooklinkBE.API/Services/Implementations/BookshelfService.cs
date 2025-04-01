using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.API.Services.Implementations
{
    public class BookshelfService(AppDbContext context) : IBookshelfService
    {
        public async Task<IEnumerable<Bookshelf>> GetBookshelvesByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID");

            return await context.Bookshelves
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Bookshelf>> GetBookshelvesByRoomIdAsync(Guid roomId)
        {
            return await context.Bookshelves.Where(x => x.RoomId == roomId).ToListAsync();
        }

        public async Task<Bookshelf?> GetBookshelfById(Guid id)
        {
            return await context.Bookshelves.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Bookshelf> CreateBookshelfAsync(CreateBookshelfModel model)
        {
            var bookshelf = new Bookshelf
            {
                Id = Guid.NewGuid(),
                UserId = model.UserId,
                Name = model.Name,
                Description = model.Description,
                NumberOfColumns = model.NumberOfColumns,
                NumberOfRows = model.NumberOfRows,
                RoomId = model.RoomId
            };

            context.Bookshelves.Add(bookshelf);
            await context.SaveChangesAsync();

            return bookshelf;
        }

        public async Task<bool> UpdateBookshelfAsync(UpdateBookshelfModel model)
        {
            var bookshelf = await context.Bookshelves.FindAsync(model.Id);
            if (bookshelf == null) return false;

            bookshelf.Name = model.Name;
            bookshelf.Description = model.Description;
            bookshelf.NumberOfColumns = model.NumberOfColumns;
            bookshelf.NumberOfRows = model.NumberOfRows;
            bookshelf.RoomId = model.RoomId;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoomAsync(Guid id)
        {
            var bookshelf = await context.Bookshelves.FindAsync(id);
            if (bookshelf == null) return false;

            context.Bookshelves.Remove(bookshelf);
            await context.SaveChangesAsync();
            return true;
        }
    }
}