using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooklinkBE.API.Controllers
{
    [ApiController]
    [Route("v1/api/bookshelves")]
    public class BookshelfController(IBookshelfService bookshelfService) : ControllerBase
    {
        [HttpGet("user:{userId:guid}")]
        public async Task<IActionResult> GetAllByUserId(Guid userId)
        {
            var bookshelves = await bookshelfService.GetBookshelvesByUserIdAsync(userId);
            return Ok(bookshelves);
        }
        
        [HttpGet("{roomId:guid}")]
        public async Task<IActionResult> GetAll(Guid roomId)
        {
            var bookshelves = await bookshelfService.GetBookshelvesByRoomIdAsync(roomId);
            return Ok(bookshelves);
        }

        [HttpGet("bookshelf/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var bookshelf = await bookshelfService.GetBookshelfById(id);
            if (bookshelf == null) return NotFound();
            return Ok(bookshelf);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookshelfModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var bookshelf = await bookshelfService.CreateBookshelfAsync(model);
            
            return CreatedAtAction(nameof(GetById), new { id = bookshelf.Id }, bookshelf);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookshelfModel model)
        {
            if (id != model.Id) return BadRequest("ID mismatch");

            var updated = await bookshelfService.UpdateBookshelfAsync(model);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await bookshelfService.DeleteRoomAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}