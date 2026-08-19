using System.Security.Claims;
using LibraryShared.classes;
using LibraryShared.dtos;
using LibraryTrackerApi.Data;
using LibraryTrackerApi.Models;
using LibraryTrackerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private AppDbContext _db;
        private readonly OpenLibraryService _openLibrary;
        private readonly BookManagerService _bookManager;

        public BooksController(AppDbContext dbContext, OpenLibraryService openLibrary, BookManagerService bookManager)
        {
            _db = dbContext;
            _openLibrary = openLibrary;
            _bookManager = bookManager;
        }

        // Get all books
        [HttpGet("books")]
        public async Task<ActionResult> GetBooks()
        {
            return Ok(await _db.Books.ToListAsync());
        }

        // Get user books
        [HttpPost("library")]
        public async Task<ActionResult<List<BookDto>>> GetUserBooksAsync(UserDto dto)
        {
            var userBooks = await _bookManager.GetUserBooksAsync(dto);
            if (userBooks is null)
                return NotFound("No user books found.");
            return userBooks;
        }

        // Get book by id
        [HttpGet("{id}")]
        public async Task<ActionResult> GetBook(Guid id)
        {
            var book = await _db.Books.FindAsync(id);

            if (book is null)
                return NotFound();

            return Ok(book);
        }
        
        // Search book by title, author and publish year
        [HttpPost("search")]
        public async Task<ActionResult<OpenLibrarySearchResponse>> SearchBookAsync(BookSearchDto dto)
        {
            var books = await _openLibrary.SearchBookAsync(dto);

            if (books is null)
                return NotFound();

            return Ok(books);
        }
        
        // Add book and bind to owner
        [HttpPost]
        public async Task<ActionResult<Book>> AddBookAsync(AddBookDto dto)
        {
            var newBook = await _bookManager.AddBookAsync(dto);

            if (newBook is null)
                return BadRequest("Something bad happened");

            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }

        // Remove a book from user library
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail) == false)
            {
                var result = await _bookManager.RemoveBookFromUserAsync(userEmail, bookId);

                if (result is null)
                    return NotFound();

                return Ok();
            }
            return NotFound();
        }
    }
}