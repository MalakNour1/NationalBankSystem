using Bank_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank_System.Configurations;

public class BranchConfigurations : IEntityTypeConfiguration<Branch> 
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ComplexProperty(b => b.Manager); //1:1 [T:T]
        builder.HasKey(b => b.Code);
    }
}