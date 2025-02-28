using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooklinkBE.Data;
using BooklinkBE.Data.Models;
    
namespace BooklinkBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.Include(r => r.RealEstate).ToListAsync();
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Room>> GetRoom(Guid id)
        {
            var room = await _context.Rooms
                .Include(r => r.RealEstate)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }

            return room;
        }
        
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            // checks if the RealEstate exists before assigning the Room
            var realEstate = await _context.RealEstates.FindAsync(room.RealEstateId);
            if (realEstate == null)
            {
                return BadRequest(new { message = "Invalid RealEstateId" });
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRoom(Guid id, Room updatedRoom)
        {
            if (id != updatedRoom.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }

            room.Name = updatedRoom.Name;
            room.RealEstateId = updatedRoom.RealEstateId;

            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
