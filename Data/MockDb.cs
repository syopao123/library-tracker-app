namespace LibraryTrackerApp.Data
{
    public class MockDb
    {

        public List<Book> Books { get; set; } = new()
        {
        new Book
        {
            Title = "Dune",
            Author = "Frank Herbert",
            Genre = "Science Fiction",
            Status = ReadingStatus.Finished,
            Rating = 5,
            DateAdded = DateTime.Now.AddDays(-30),
            PersonalNotes = "Reread this every couple of years.",
            ImageUrl = "https://m.media-amazon.com/images/S/compressed.photo.goodreads.com/books/1555447414i/44767458.jpg"
        },
        new Book
        {
            Title = "Project Hail Mary",
            Author = "Andy Weir",
            Genre = "Science Fiction",
            Status = ReadingStatus.Reading,
            Rating = null,
            DateAdded = DateTime.Now.AddDays(-5),
            ImageUrl = "https://images.cdn3.buscalibre.com/fit-in/660x660/db/31/db3163b496e917f895d8e42e180f24dc.jpg"
        },
        new Book
        {
            Title = "The Name of the Wind",
            Author = "Patrick Rothfuss",
            Genre = "Fantasy",
            Status = ReadingStatus.ToRead,
            Rating = null,
            DateAdded = DateTime.Now,
            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/5/56/TheNameoftheWind_cover.jpg?utm_source=en.wikipedia.org&utm_campaign=index&utm_content=original"
        }
        };
        
    }
}