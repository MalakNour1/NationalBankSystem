using System.ComponentModel.DataAnnotations;

namespace Bank_System.Models;

public class Customer : User
{
    public int Id { get; set; }
    public DateTime DateOfBirth { get; set; }
    [MaxLength(14)]
    public string NationalId { get; set; } = null!;
    [MaxLength(50)]
    public string Address { get; set; } = null!;
    [MaxLength(10)]
    public string CustomerType { get; set; } = null!;
    
    public ICollection<CustomerAccount> Accounts { get; set; } = new HashSet<CustomerAccount>();
}