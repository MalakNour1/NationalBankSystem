using Bank_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank_System.Configurations;

public class TransactionConfigurations : IEntityTypeConfiguration<Transaction> 
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.TransactionNumber);
    }
}