using LibraryShared.dtos;
using LibraryTrackerApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryTrackerApi.Services
{
    public class BrowserService
    {
        private readonly AppDbContext _db;

        public BrowserService(AppDbContext db)
        {
            _db = db;            
        }

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
        
    }
}