using ImageManipulationApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageManipulationApi.Database
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext() {}

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Set in appsettings.json => unnecasery
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\\mssqllocaldb;Database=ImageManipulationDb;Integrated Security=True");
            }
        }

        public DbSet<UserManipulatedImage> UserManipulatedImages { get; set; }
    }
}
