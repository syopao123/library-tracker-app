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
            PersonalNotes = "Reread this every couple of years."
        },
        new Book
        {
            Title = "Project Hail Mary",
            Author = "Andy Weir",
            Genre = "Science Fiction",
            Status = ReadingStatus.Reading,
            Rating = null,
            DateAdded = DateTime.Now.AddDays(-5)
        },
        new Book
        {
            Title = "The Name of the Wind",
            Author = "Patrick Rothfuss",
            Genre = "Fantasy",
            Status = ReadingStatus.ToRead,
            Rating = null,
            DateAdded = DateTime.Now
        }
        };
        
    }
}