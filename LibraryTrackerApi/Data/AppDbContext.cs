using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryTrackerApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryTrackerApi.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookOwner> BookOwners { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BookOwner>().HasKey(bo => new { bo.UserId, bo.BookId });
            builder.Entity<Book>().HasMany(b => b.BookOwners).WithOne(bo => bo.Book).HasForeignKey(bo => bo.BookId);
            builder.Entity<User>().HasMany(b => b.BookOwners).WithOne(bo => bo.User).HasForeignKey(bo => bo.UserId);
        }
        
    }
}