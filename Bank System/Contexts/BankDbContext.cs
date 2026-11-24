using System.Reflection;
using Bank_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_System.Contexts;

public class BankDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-EV6VD57\\MSSQLSERVER01;" +
            "Database=NationalBankDB;" +
            "Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Branch> Branches { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CustomerAccount> CustomerAccounts { get; set; }
}