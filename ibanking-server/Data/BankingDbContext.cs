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

        public DbSet<Account> Accounts { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
