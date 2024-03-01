using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using tution_service.Models;

namespace tution_service.Data
{
    public class TutionDbContext : DbContext
    {
      

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer();
        //}

        public TutionDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Tution> Tutions { get; set; }
    }
}
