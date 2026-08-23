using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using LibraryShared.dtos;
using LibraryShared.enums;
using LibraryTrackerApi.Data;
using LibraryTrackerApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LibraryTrackerApi.Services
{
    public class BookManagerService
    {
        private AppDbContext _db;
        private UserManager<User> _userManager;

        public BookManagerService(AppDbContext dbContext, UserManager<User> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }

        public async Task<Book?> GetBookAsync(Guid id) => await _db.Books.FindAsync(id);

        // Checks if book exists in db
        private async Task<Book> CheckIfBookExistsAsync(AddBookDto dto)
        {
            var book = await _db.Books
                .Where(b => b.OpenLibraryKey == dto.OpenLibraryKey)
                .FirstOrDefaultAsync();

            if (book is not null) return book;

            // Create new Book then store in db if it doesn't exist yet
            Book newBook = new()
            {
                OpenLibraryKey = dto.OpenLibraryKey,
                Title = dto.Title,
                AuthorNames = dto.AuthorNames,
                Subjects = dto.Subjects,
                ImageUrl = $"https://covers.openlibrary.org/b/id/{dto.CoverI}-L.jpg"
            };
            await _db.Books.AddAsync(newBook);
            return newBook;
        }

        // Returns null if book has not been added to user's library yet
        private async Task<BookOwner?> CheckIfUserAlreadyOwnsBookAsync(string userEmail, Book book)
        {
            return await _db.BookOwners.Where(bo => bo.User.Email == userEmail && bo.BookId == book.Id).FirstOrDefaultAsync();
        }

        // Add book to db and bind to user using BookOwner
        public async Task<(bool, string, BookDto?)> AddBookAsync(string userEmail, AddBookDto dto)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null) return (false, "User not found.", null);

            var dbBook = await CheckIfBookExistsAsync(dto);

            var bookOwner = await CheckIfUserAlreadyOwnsBookAsync(userEmail, dbBook);
            if (bookOwner is not null) return (false, "You already added the book to your library.", null);

            var newBookOwner = new BookOwner()
            {
                UserId = user.Id,
                Book = dbBook,
                User = user,
                Status = dto.Status,
                Rating = dto.Rating,
                PersonalNotes = dto.PersonalNotes,
                CustomAuthor = dto.CustomAuthor,
                CustomGenre = dto.CustomGenre
            };

            await _db.BookOwners.AddAsync(newBookOwner);
            await _db.SaveChangesAsync();

            var bookDto = new BookDto() { Id = dbBook.Id, Title = dbBook.Title, CustomAuthor = newBookOwner.CustomAuthor };
            return (true, "Book has been added to user's library.", bookDto);
        }

        public async Task<List<BookDto>?> GetUserBooksAsync(string userEmail)
        {
            var userExists = await _userManager.Users.AnyAsync(u => u.Email == userEmail);
            if (userExists == false) return null;

            // Find books belonging to user
            var userBooks = await _db.BookOwners.Where(bo => bo.User.Email == userEmail)
            .Select(bo => new BookDto()
            {
                Id = bo.Book.Id,
                Title = bo.Book.Title,
                CustomAuthor = bo.CustomAuthor,
                ImageUrl = bo.Book.ImageUrl,
                CustomGenre = bo.CustomGenre,
                Status = bo.Status,
                Rating = bo.Rating,
                PersonalNotes = bo.PersonalNotes,
                Subjects = bo.Book.Subjects,
                AuthorName = bo.Book.AuthorNames
            }).ToListAsync();

            return userBooks;
        }

        // Fetches then updates book details
        public async Task<(bool, string)> UpdateUserBookAsync(string userEmail, UpdateBookDto dto)
        {
            var userExists = await _userManager.Users.AnyAsync(u => u.Email == userEmail);
            if (userExists == false) return (false, "User not found.");

            var bookOwner = await _db.BookOwners.Include(bo => bo.Book).Where(bo => bo.User.Email == userEmail && bo.BookId == dto.Id).FirstOrDefaultAsync();
            if (bookOwner is null) return (false, "User does not own the given book.");

            bookOwner.CustomTitle = dto.CustomTitle ?? bookOwner.Book.Title;
            bookOwner.CustomAuthor = dto.CustomAuthor ?? bookOwner.CustomAuthor;
            bookOwner.CustomImageUrl = dto.CustomImageUrl ?? bookOwner.Book.ImageUrl;
            bookOwner.CustomGenre = dto.CustomGenre;
            bookOwner.Status = dto.UpdatedStatus;
            bookOwner.Rating = dto.UpdatedRating;
            bookOwner.PersonalNotes = dto.UpdatedPersonalNotes;

            var result = await _db.SaveChangesAsync();
            return result > 0 ? (true, "User successfully updated book details.") : (true, "No update was made.");
        }

        // Remove book from user's library
        public async Task<BookOwner?> RemoveBookFromUserAsync(string userEmail, Guid bookId)
        {
            var userExists = await _userManager.Users.AnyAsync(u => u.Email == userEmail);
            if (userExists == false) return null;

            var bookOwner = await _db.BookOwners.Where(bo => bo.User.Email == userEmail && bo.BookId == bookId).FirstOrDefaultAsync();
            if (bookOwner is null) return null;

            _db.BookOwners.Remove(bookOwner);
            var result = await _db.SaveChangesAsync();

            return bookOwner;
        }
    }
}