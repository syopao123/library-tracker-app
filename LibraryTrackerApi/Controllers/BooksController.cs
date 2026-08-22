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
        private readonly OpenLibraryService _openLibrary;
        private readonly BookManagerService _bookManager;

        private string? UserEmail => User.FindFirstValue(ClaimTypes.Email);

        public BooksController(OpenLibraryService openLibrary, BookManagerService bookManager)
        {
            _openLibrary = openLibrary;
            _bookManager = bookManager;
        }

        // Get user books
        [HttpGet("library")]
        public async Task<ActionResult<List<BookDto>>> GetUserBooksAsync()
        {
            if (string.IsNullOrEmpty(UserEmail)) return BadRequest();

            var userBooks = await _bookManager.GetUserBooksAsync(UserEmail);

            if (userBooks is null) return NotFound();

            return Ok(userBooks);
        }

        // Get book by id
        [HttpGet("{id}")]
        public async Task<ActionResult> GetBookAsync(Guid id)
        {
            if (string.IsNullOrEmpty(UserEmail))
                return BadRequest();
                
            var result = await _bookManager.GetBookAsync(id);

            if (result is null)
                return NotFound();

            return Ok(result);
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
            if (string.IsNullOrEmpty(UserEmail)) return BadRequest();

            var (result, message, book) = await _bookManager.AddBookAsync(UserEmail, dto);

            if (result == false)
                return Conflict(message);
            
            return CreatedAtAction("GetBook", new { id = book!.Id }, book);
        }

        // Update user's book details
        [HttpPatch]
        public async Task<ActionResult> UpdateUserBookAsync(UpdateBookDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail)) return BadRequest();

            var (isSuccess, message) = await _bookManager.UpdateUserBookAsync(UserEmail, dto);

            if (isSuccess == false) return BadRequest(message);

            return NoContent();
        }

        // Delete book by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUserBookAsync(Guid id)
        {
            if (string.IsNullOrEmpty(UserEmail)) return BadRequest();

            var result = await _bookManager.RemoveBookFromUserAsync(UserEmail, id);

            if (result is null) return NotFound();

            return Ok();
        }
    }
}