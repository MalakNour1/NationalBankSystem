using System.ComponentModel.DataAnnotations;

namespace Bank_System.Models;

public class Branch
{
    public int Code { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    [MaxLength(50)]
    public string Address { get; set; } = null!;
    [MaxLength(11)]
    public string PhoneNumber { get; set; } = null!;
    public Manager Manager { get; set; } = null!; //complex property
    
    public ICollection<Account>  Accounts { get; set; } = new HashSet<Account>();
}