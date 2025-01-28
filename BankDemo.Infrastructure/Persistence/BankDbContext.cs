using Microsoft.EntityFrameworkCore;
using BankDemo.Domain.Account;

   namespace BankDemo.Infrastructure.Persistence
   {
       public class BankDbContext : DbContext
       {
           public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
           {
           }

           public DbSet<Account> Accounts { get; set; } = null!;
       }
   }