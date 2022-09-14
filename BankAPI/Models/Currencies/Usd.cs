namespace BankAPI.Models.Currencies;

public class Usd : Currencies.Currency
{
    private const string _name = "USD";
    public Usd() : base(Usd._name)
    {

    }
}
