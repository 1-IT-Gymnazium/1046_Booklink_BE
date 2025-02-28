using System.Security.Claims;
using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooklinkBE.API.Controllers
{
    [ApiController]
    [Route("v1/api/[controller]")]
    public class BookController(IBookService bookService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetUserBooks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var books = await bookService.GetUserBooksAsync(Guid.Parse(userId));
            return Ok(books);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Book>> GetBook(Guid id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(CreateBookRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(userId);
            if (userId == null)
            {
                return Unauthorized();
            }

            var createdBook = await bookService.CreateBookAsync(Guid.Parse(userId), request);
            
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBook(Guid id, UpdateBookRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var updatedBook = await bookService.UpdateBookAsync(id, request);
            if (updatedBook == null)
            {
                return NotFound();
            }

            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await bookService.DeleteBookAsync(id);
            return NoContent();
        }
    }
}