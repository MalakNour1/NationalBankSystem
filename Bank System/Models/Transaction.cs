using System.ComponentModel.DataAnnotations;

namespace Bank_System.Models;

public class Transaction
{
    public int TransactionNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    [MaxLength(100)]
    public string Note { get; set; } = null!;
    [MaxLength(50)]
    public string TransactionType { get; set; } = null!;
    
    public int AccountId {get; set;}
    public Account Account { get; set; } = null!;
}