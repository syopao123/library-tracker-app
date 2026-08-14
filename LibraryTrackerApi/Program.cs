using System.Text.Json;
using LibraryShared.dtos;
using LibraryTrackerApi.Data;
using LibraryTrackerApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorization();

builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.MapGet("/books", (AppDbContext db) =>
{
    return db.Books.ToListAsync();
});

app.MapPost("/add-book", async (BookDto bookDto, AppDbContext db) =>
{
    // Check if book title exists
    var result = await db.Books.Where(b => b.Title.ToUpper().Equals(bookDto.Title.ToUpper())).ToListAsync();

    // Return the books from db matching the user's book
    if (result.Count() > 0)
        return Results.Ok(result);

    // var book = new Book();

    // db.Books.Add(book);
    //await db.SaveChangesAsync();
    //return Results.Created($"You successfully added {book.Title} by {book.Author}.", book);
    return Results.Ok("test");
});


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ASP.NET Core Identity endpoints
app.MapIdentityApi<User>();

app.Run();