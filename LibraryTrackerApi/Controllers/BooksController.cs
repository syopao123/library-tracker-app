using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using LibraryShared.classes;
using LibraryShared.dtos;
using LibraryShared.enums;
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

        private string? UserEmail => User.FindFirstValue(ClaimTypes.Email);

        public BooksController(AppDbContext dbContext, OpenLibraryService openLibrary, BookManagerService bookManager)
        {
            _db = dbContext;
            _openLibrary = openLibrary;
            _bookManager = bookManager;
        }

        // Get user books
        [HttpGet("library")]
        public async Task<ActionResult<List<BookDto>>> GetUserBooksAsync()
        {
            if (string.IsNullOrEmpty(UserEmail))
                return BadRequest();

            var userBooks = await _bookManager.GetUserBooksAsync(UserEmail);                
            return Ok(userBooks);
        }

        // Get book by id
        [HttpGet("{id}")]
        public async Task<ActionResult> GetBook(Guid id)
        {
            if (string.IsNullOrEmpty(UserEmail))
                return BadRequest();
                
            var book = await _db.Books.FindAsync(id);

            if (book is null)
                return NotFound();

            return Ok(book);
        }
        
        // Search book by title, author and publish year
        [HttpPost("search")]
        public async Task<ActionResult<OpenLibrarySearchResponse>> SearchBookAsync(BookSearchDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail))
                return BadRequest();

            var books = await _openLibrary.SearchBookAsync(dto);

            if (books is null)
                return NotFound();

            return Ok(books);
        }
        
        // Add book and bind to owner
        [HttpPost]
        public async Task<ActionResult<BookDto>> AddBookAsync(AddBookDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail))
                return BadRequest();

            var (result, book) = await _bookManager.AddBookAsync(UserEmail, dto);

            if (result == AddBookResult.UserNotFound)
                return BadRequest("User not found.");
            else if (result == AddBookResult.UserAlreadyOwnsBook)
                return Conflict("User already owns the book.");
            
            return CreatedAtAction(nameof(GetBook), new { id = book!.Id }, book);
        }

        // Delete book by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            if (string.IsNullOrEmpty(UserEmail))
                return BadRequest();

            var result = await _bookManager.RemoveBookFromUserAsync(UserEmail, id);

            if (result is null)
                return NotFound();

            return Ok();
        }
    }
}