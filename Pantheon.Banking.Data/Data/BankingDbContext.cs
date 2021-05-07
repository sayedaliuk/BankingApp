using Microsoft.EntityFrameworkCore;

namespace Pantheon.Banking.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions options) : base(options)
        {
        }        

        public DbSet<BankAccount> Account { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }
        public DbSet<AccountType> AccountType { get; set; }
    }
}
