using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.API.Services.Implementations
{
    public class BookshelfService : IBookshelfService
    {
        private readonly AppDbContext _context;

        public BookshelfService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bookshelf>> GetAllAsync()
        {
            return await _context.Bookshelves.Include(b => b.Room).ToListAsync();
        }

        public async Task<Bookshelf?> GetByIdAsync(Guid id)
        {
            return await _context.Bookshelves.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Bookshelf> CreateAsync(CreateBookshelfModel model)
        {
            var bookshelf = new Bookshelf
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                NumberOfColumns = model.NumberOfColumns,
                NumberOfRows = model.NumberOfRows,
                RoomId = model.RoomId
            };

            _context.Bookshelves.Add(bookshelf);
            await _context.SaveChangesAsync();

            return bookshelf;
        }

        public async Task<bool> UpdateAsync(UpdateBookshelfModel model)
        {
            var bookshelf = await _context.Bookshelves.FindAsync(model.Id);
            if (bookshelf == null) return false;

            bookshelf.Name = model.Name;
            bookshelf.Description = model.Description;
            bookshelf.NumberOfColumns = model.NumberOfColumns;
            bookshelf.NumberOfRows = model.NumberOfRows;
            bookshelf.RoomId = model.RoomId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var bookshelf = await _context.Bookshelves.FindAsync(id);
            if (bookshelf == null) return false; //404

            _context.Bookshelves.Remove(bookshelf);
            await _context.SaveChangesAsync();
            return true; //204
        }
    }
}
