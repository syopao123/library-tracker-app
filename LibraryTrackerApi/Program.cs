using LibraryTrackerApi.Data;
using LibraryTrackerApi.Models;
using LibraryTrackerApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Scalar.AspNetCore;

var blazorClient = "blazorWasmClient";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    // LibraryTrackerApp frontend
    options.AddPolicy(name: blazorClient, policy =>
    {
        policy.WithOrigins("http://localhost:5051").AllowAnyMethod().AllowAnyHeader();
    });
});

// OpenLibrary Book Search API
builder.Services.AddHttpClient("BookSearchApi", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://openlibrary.org/search.json?");
    httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, builder.Configuration["UserAgent:OpenLibApi"]);
});

builder.Services.AddSingleton<OpenLibraryService>();
builder.Services.AddScoped<BookManagerService>();
builder.Services.AddScoped<BrowserService>();

builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = false);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors(blazorClient);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ASP.NET Core Identity endpoints
app.MapIdentityApi<User>();
app.MapControllers();

app.Run();