using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booksClub.Models
{
    public class BooksClubContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
        public BooksClubContext(DbContextOptions<BooksClubContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
