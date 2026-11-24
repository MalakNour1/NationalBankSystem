using System.ComponentModel.DataAnnotations;

namespace Bank_System.Models;

public class Account
{
    public int AccountNumber { get; set; }
    public decimal CurrentBalance { get; set; }
    [MaxLength(50)]
    public string AccountType { get; set; } = null!;
    public DateTime OpeningDate { get; set; }
    
    
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    
    public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    public ICollection<CustomerAccount> Customers { get; set; } = new HashSet<CustomerAccount>();
}