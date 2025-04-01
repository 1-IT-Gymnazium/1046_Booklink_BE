using Microsoft.AspNetCore.Mvc;
using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data.Models;

namespace BooklinkBE.API.Controllers
{
    [Route("v1/api/rooms")]
    [ApiController]
    public class RoomsController(IRoomService roomService) : ControllerBase
    {
        [HttpGet("user:{userId:guid}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsByUserId(Guid userId)
        {
            var rooms = await roomService.GetRoomsByUserIdAsync(userId);
            return Ok(rooms);
        }
        [HttpGet("householdId:{id:guid}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms(Guid id)
        {
            var rooms = await roomService.GetRoomsAsync(id);
            return Ok(rooms);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Room>> GetRoom(Guid id)
        {
            var room = await roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }
            return Ok(room);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom([FromBody] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                HouseholdId = request.HouseholdId,
                UserId = request.UserId,
            };

            try
            {
                var newRoom = await roomService.CreateRoomAsync(room);
                return CreatedAtAction(nameof(GetRoom), new { id = newRoom.Id }, newRoom);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomRequest request)
        {
            var updated = await roomService.UpdateRoomAsync(id, request);
            if (!updated)
            {
                return NotFound(new { message = "Room not found" });
            }

            return NoContent();
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            try
            {
                await roomService.DeleteRoomAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
