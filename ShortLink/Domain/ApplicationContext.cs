using Microsoft.EntityFrameworkCore;
using ShortLink.Models;

namespace ShortLink.Domain
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<RecordViewModel> Records { get; set; }
    }
}
