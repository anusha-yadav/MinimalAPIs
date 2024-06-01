using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    /// <summary>
    /// DBContext file
    /// </summary>
    public class LibraryDBContext : DbContext
    {
        public LibraryDBContext() { }
        public LibraryDBContext(DbContextOptions<LibraryDBContext> options) : base(options) { }

        public DbSet<Books> Books { get; set; }

        public DbSet<Author> Author { get; set; }
    }
}
