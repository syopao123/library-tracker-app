using LibraryShared.dtos;
using LibraryTrackerApi.Data;
using LibraryTrackerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryTrackerApi.Services
{
    public class BrowserService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;

        public BrowserService(AppDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Returns books added by users recently
        public async Task<List<CommunityBookDto>> GetCommunityBooksAsync()
        {
            var communityBooks = await _db.BookOwners.OrderByDescending(bo => bo.DateAdded).Select(bo => new CommunityBookDto
            {
                BookId = bo.BookId,
                Username = bo.User.UserName ?? "User",
                Title = bo.Book.Title,
                DateAdded = bo.DateAdded

            }).Take(5).ToListAsync();

            return communityBooks;
        }

        public async Task<List<PopularBookDto>> GetPopularBooksAsync(int count, string userEmail)
        {
            string userId = _userManager.FindByEmailAsync(userEmail).Result!.Id;

            var popularBooks = await _db.BookOwners
            .GroupBy(bo => new { bo.BookId, bo.Book.OpenLibraryKey, bo.UserId, bo.Book.Title, bo.Book.AuthorNames, bo.Book.ImageUrl } )
            .Select(bo => new PopularBookDto
            {
                OpenLibraryKey = bo.Key.OpenLibraryKey,
                Id = bo.Key.BookId,
                Title = bo.Key.Title,
                AuthorNames = bo.Key.AuthorNames,
                ImageUrl = bo.Key.ImageUrl,
                OwnerCount = bo.Count(),
                IsOwnedByUser = bo.Key.UserId.Equals(userId)
            })
            .OrderByDescending(bo => bo.OwnerCount)
            .Take(count)
            .ToListAsync();

            return popularBooks;
        }
        
    }
}