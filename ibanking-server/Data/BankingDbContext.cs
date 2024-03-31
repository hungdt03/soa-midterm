using ibanking_server.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ibanking_server.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.OTP)
                .WithOne(o => o.Transaction)
                .HasForeignKey<OTP>(o => o.TransactionId);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
