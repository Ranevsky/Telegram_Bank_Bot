
using BankAPI.Models.Currencies;

namespace BankAPI.Models;

public class Department
{
    public int Id { get; set; }
    public string Street { get; set; } = null!;
    public Location? Location { get; set; }
    public List<Currency> Currencies { get; set; } = new();
    public City City { get; set; } = null!;
}
