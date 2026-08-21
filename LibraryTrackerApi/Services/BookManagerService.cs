using System.Diagnostics;
using LibraryShared.dtos;
using LibraryShared.enums;
using LibraryTrackerApi.Data;
using LibraryTrackerApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // Checks if book exists in db, if not then add it
        private async Task<Book> CheckIfBookExistsAsync(AddBookDto dto)
        {
            var book = await _db.Books
                .Where(b => b.OpenLibraryKey.Equals(dto.OpenLibraryKey))
                .FirstOrDefaultAsync();

            if (book is null)
            {
                Book newBook = new()
                {
                    OpenLibraryKey = dto.OpenLibraryKey,
                    Title = dto.Title,
                    Author = dto.Author,
                    Genre = dto.Genre,
                    ImageUrl = $"https://covers.openlibrary.org/b/id/{dto.CoverI}-L.jpg"
                };
                return newBook;
            }
            return book;
        }

        private async Task<BookOwner?> CheckIfUserAlreadyOwnsBookAsync(User user, Book book)
        {
            var bookOwner = await _db.BookOwners.Where(bo => bo.UserId == user.Id && bo.BookId == book.Id).FirstOrDefaultAsync();
            return bookOwner;
        }

        // Add book to db and bind to user using BookOwner
        public async Task<(AddBookResult, BookDto?)> AddBookAsync(string userEmail, AddBookDto dto)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            var dbBook = await CheckIfBookExistsAsync(dto);

            if (user is not null)
            {
                var bookDto = new BookDto() { Id= dbBook.Id, Title = dbBook.Title, Author = dbBook.Author };

                var bookOwnerCheck = await CheckIfUserAlreadyOwnsBookAsync(user, dbBook);

                // Only create book ownership if user doesnt own the book
                if (bookOwnerCheck == null)
                {
                    var newBookOwner = new BookOwner()
                    {
                        UserId = user.Id,
                        Book = dbBook,
                        User = user,
                        Status = dto.Status,
                        Rating = dto.Rating,
                        PersonalNotes = dto.PersonalNotes,
                    };

                    await _db.BookOwners.AddAsync(newBookOwner);
                    await _db.SaveChangesAsync();
                    
                    return (AddBookResult.BookAddedToLibrary, bookDto);
                }
                return (AddBookResult.UserAlreadyOwnsBook, bookDto);
            }
            return (AddBookResult.UserNotFound, null);
        }

        public async Task<List<BookDto>?> GetUserBooksAsync(string userEmail)
        {
            // Find user
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user is not null)
            {
                List<BookDto>? userBooks = new();

                // Find book ownerships
                var bookOwnership = await _db.BookOwners.Where(bo => bo.UserId == user.Id).ToListAsync();

                // Find books per ownership
                foreach (var ownership in bookOwnership)
                {
                    var book = await _db.Books.FindAsync(ownership.BookId);

                    // If book is found, store it inside dto then add it to userBooks
                    if (book is not null)
                    {
                        var bookDto = new BookDto()
                        {
                            Id = book.Id,
                            Title = book.Title,
                            Author = book.Author,
                            // TODO: Assign proper image url upon book add
                            ImageUrl = book.ImageUrl,
                            Genre = book.Genre ?? "",
                            Status = ownership.Status,
                            Rating = ownership.Rating,
                            PersonalNotes = ownership.PersonalNotes ?? "",
                            // TODO: Add FirstPublishYear & PublishYears to Book model
                        };
                        userBooks.Add(bookDto);
                    }
                }
                // Return books to user
                return userBooks;
            }
            return null;
        }

        // Remove book from user's library
        public async Task<BookOwner?> RemoveBookFromUserAsync(string userEmail, Guid bookId)
        {
            // Find user
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user is not null)
            {
                // Find book
                var book = await _db.Books.FindAsync(bookId);

                if (book is not null)
                {
                    var bookOwner = await _db.BookOwners.Where(bo => bo.BookId.Equals(book.Id) && bo.UserId.Equals(user.Id)).FirstAsync();
                    _db.BookOwners.Remove(bookOwner);
                    var result = await _db.SaveChangesAsync();

                    if (result > 0)
                        return bookOwner;
                }
            }
            return null;
        }
    }
}