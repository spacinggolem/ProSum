using Microsoft.EntityFrameworkCore;

namespace ProSum.Models.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // public DbSet<Movie> Movie { get; set; }
    }
}
