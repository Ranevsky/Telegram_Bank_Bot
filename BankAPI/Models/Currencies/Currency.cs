namespace BankAPI.Models.Currencies;

public class Currency
{
    public Currency(string name)
    {
        Name = name;
    }
    public Currency()
    {

    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Buy { get; set; }
    public decimal Sell { get; set; }
}
