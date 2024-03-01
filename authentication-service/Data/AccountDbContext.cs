using authentication_service.Models;
using Microsoft.EntityFrameworkCore;

namespace authentication_service.Data
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
