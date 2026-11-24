using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace Bank_System.Models;

[PrimaryKey(nameof(CustomerId) , nameof(AccountId))]
public class CustomerAccount
{
    // Customer [1 =M] CustomerAccount [M= 1] Account
    [MaxLength(50)]
    public string OwnershipType { get; set; } = null!;
    public DateTime OwnershipStartDate { get; set; }
    [MaxLength(50)]
    public string AccountStatus { get; set; } = null!;
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    public int AccountId { get; set; }
    public Account Account { get; set; }
}