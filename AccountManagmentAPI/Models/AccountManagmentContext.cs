using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountManagmentAPI.Models
{
    public class AccountManagmentContext : IdentityDbContext
    {
        public AccountManagmentContext(DbContextOptions<AccountManagmentContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
    }

}
