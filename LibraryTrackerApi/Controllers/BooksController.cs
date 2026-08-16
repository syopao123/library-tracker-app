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

        public BooksController(AppDbContext dbContext, OpenLibraryService openLibrary)
        {
            _db = dbContext;
            _openLibrary = openLibrary;
        }

        [HttpPost("search")]
        public async Task<ActionResult<OpenLibrarySearchResponse>> SearchBookAsync(BookSearchDto dto)
        {
            var books = await _openLibrary.SearchBookAsync(dto);

            if (books is null)
                return NotFound();

            return Ok(books);
        }


        [HttpGet("books")]
        public async Task<ActionResult> GetBooks()
        {
            return Ok(await _db.Books.ToListAsync());
        }
        

        [HttpGet("{id}")]
        public async Task<ActionResult> GetBook(Guid id)
        {
            var book = await _db.Books.FindAsync(id);

            if (book is null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(BookDto dto)
        {
            var newBook = new Book()
            {
                Title = dto.Title,
                Author = dto.Author,
            };

            await _db.Books.AddAsync(newBook);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(Guid id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book is null)
            {
                return NotFound("Book Id not found");
            }
            else
            {
                _db.Books.Remove(book);
                await _db.SaveChangesAsync();
                return Ok("Book has been deleted");
            }
        }
    }
}