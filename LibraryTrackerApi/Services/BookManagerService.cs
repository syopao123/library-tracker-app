using System.Diagnostics;
using LibraryShared.dtos;
using LibraryShared.enums;
using LibraryTrackerApi.Data;
using LibraryTrackerApi.Models;
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

        /*
            TODO:
                1. Check if added book already exists inside Books
                2. Check if user already added that book to their library
        */
        // Add to Books then create BookOwner data
        public async Task<Book?> AddBookAsync(AddBookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserEmail) == false)
            {
                User? user = await _userManager.FindByEmailAsync(dto.UserEmail);

                if (user is not null)
                {
                    var newBook = new Book()
                    {
                        Title = dto.Title,
                        Author = dto.Author,
                        Genre = dto.Genre,
                        ImageUrl = $"https://covers.openlibrary.org/b/id/{dto.CoverI}-L.jpg"
                    };

                    var newBookOwner = new BookOwner()
                    {
                        UserId = user.Id,
                        Book = newBook,
                        User = user,
                        Status = dto.Status,
                        Rating = dto.Rating,
                        PersonalNotes = dto.PersonalNotes,
                    };

                    await _db.BookOwners.AddAsync(newBookOwner);
                    var result = await _db.SaveChangesAsync();

                    if (result > 0)
                    {
                        return newBook;
                    }
                }
            }
            return null;
        }

        public async Task<List<BookDto>?> GetUserBooksAsync(UserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) == false)
            {
                // Find user
                var user = await _userManager.FindByEmailAsync(dto.Email);

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
            }
            return null;
        }

        // Remove book from user's library
        public async Task<Book?> RemoveBookFromUserAsync(string userEmail, Guid bookId)
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
                        return book;
                }
            }
            return null;
        }
    }
}