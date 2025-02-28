using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooklinkBE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookshelfController : ControllerBase
    {
        private readonly IBookshelfService _bookshelfService;

        public BookshelfController(IBookshelfService bookshelfService)
        {
            _bookshelfService = bookshelfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookshelves = await _bookshelfService.GetAllAsync();
            return Ok(bookshelves);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var bookshelf = await _bookshelfService.GetByIdAsync(id);
            if (bookshelf == null) return NotFound();
            return Ok(bookshelf);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookshelfModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var bookshelf = await _bookshelfService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = bookshelf.Id }, bookshelf);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookshelfModel model)
        {
            if (id != model.Id) return BadRequest("ID mismatch");

            var updated = await _bookshelfService.UpdateAsync(model);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _bookshelfService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}