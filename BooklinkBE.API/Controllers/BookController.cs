using System.Security.Claims;
using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooklinkBE.API.Controllers;

[ApiController]
[Route("v1/api/books")]
public class BookController(IBookService bookService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IEnumerable<Book>>> GetUserBooks(Guid id)
    {
        if (id == null) {
            return Unauthorized();
        }

        var books = await bookService.GetUserBooksAsync(id);
        return Ok(books);
    }
        
    [HttpGet("bookshelf:{id:guid}")]
    public async Task<IActionResult> GetBooksByBookshelfId(Guid id)
    {
        var book = await bookService.GetBooksByBookshelfIdAsync(id);
        if (book == null) return NotFound();
        return Ok(book);
    }
        
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
            
        var createdBook = await bookService.CreateBookAsync(request);
            
        return CreatedAtAction(nameof(GetBooksByBookshelfId), new { id = createdBook.Id }, createdBook);
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
        try
        {
            await bookService.DeleteBookAsync(id);
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