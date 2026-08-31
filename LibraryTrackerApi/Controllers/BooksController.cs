using System.Security.Claims;
using LibraryShared.classes;
using LibraryShared.dtos;
using LibraryTrackerApi.Models;
using LibraryTrackerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly OpenLibraryService _openLibrary;
        private readonly BookManagerService _bookManager;
        private readonly BrowserService _browserService;

        private string? UserEmail => User.FindFirstValue(ClaimTypes.Email);

        public BooksController(OpenLibraryService openLibrary, BookManagerService bookManager, BrowserService browserService)
        {
            _openLibrary = openLibrary;
            _bookManager = bookManager;
            _browserService = browserService;
        }

        // Get user books
        [HttpGet("library")]
        public async Task<ActionResult<List<BookDto>>> GetUserBooksAsync()
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var userBooks = await _bookManager.GetUserBooksAsync(UserEmail);

            if (userBooks is null) return NotFound();

            return userBooks;
        }

        // Get book by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookAsync(Guid id)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();
                
            var book = await _bookManager.GetBookAsync(id);

            if (book is null) return NotFound();

            return book;
        }
        
        // Search book by title, author and publish year
        [HttpPost("search")]
        public async Task<ActionResult<OpenLibrarySearchResponse>> SearchBookAsync(BookSearchDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var searchedBooks = await _openLibrary.SearchBookAsync(dto);

            if (searchedBooks is null)
                return NotFound();

            return searchedBooks;
        }
        
        // Add book and bind to owner
        [HttpPost]
        public async Task<ActionResult<BookDto>> AddBookAsync(AddBookDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var (result, message, book) = await _bookManager.AddBookAsync(UserEmail, dto);

            if (result == false)
                return Conflict(message);
            
            return CreatedAtAction("GetBook", new { id = book!.Id }, book);
        }

        // Update user's book details
        [HttpPatch]
        public async Task<IActionResult> UpdateUserBookAsync(UpdateBookDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var (isSuccess, message) = await _bookManager.UpdateUserBookAsync(UserEmail, dto);

            if (isSuccess == false) return BadRequest(message);

            return NoContent();
        }

        // Delete book by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUserBookAsync(Guid id)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var result = await _bookManager.RemoveBookFromUserAsync(UserEmail, id);

            if (result is null) return NotFound();

            return Ok();
        }

        // Gets latest books added by users
        [HttpGet("community")]
        public async Task<ActionResult<List<CommunityBookDto>>> GetCommunityBooksAsync()
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var communityBooks = await _browserService.GetCommunityBooksAsync();

            return communityBooks;
        }

        [HttpGet("popular-books/{count}")]
        public async Task<ActionResult<List<PopularBookDto>>> GetPopularBooksAsync(int count)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();
            
            var popularBooks = await _browserService.GetPopularBooksAsync(count, UserEmail);

            return popularBooks;
        }
    }
}