using Microsoft.AspNetCore.Mvc;
using Booklink.Models;

namespace Booklink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        // Konstruktor kontroleru, kde je injektován repozitář knih
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // Asynchronní metoda pro získání seznamu všech knih
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            return Ok(books);   // Vrací HTTP status 200 s načtenými knihami
        }

        // Asynchronní metoda pro získání konkrétní knihy podle ID
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Book>> GetBook(Guid id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound(); // kniha nenalezena
            }
            return Ok(book);  // Vrací HTTP status 200 => kniha nalezena
        }

        // metoda pro vytvoření a asynchronní přidání nové knihy
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            var createdBook = await _bookRepository.AddBookAsync(book);
            
            // Vrací HTTP status 201 (Created) s odkazem na nově vytvořenou knihu
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        // metoda pro asynchronní aktualizaci knihy s kontrolou ID v URL s ID v těle požadavku
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBook(Guid id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            await _bookRepository.UpdateBookAsync(book);
            return NoContent(); // Vrací HTTP status 204 => úspěšná aktualizace dat
        }

        // HTTP DELETE metoda pro asynchronní odstranění knihy podle ID
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookRepository.DeleteBookAsync(id);
            return NoContent(); // Vrací HTTP status 204 => úspěšné odstranění knihy
        }
    }
}