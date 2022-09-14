namespace BankAPI.Models.Currencies;

public class Eur : Currencies.Currency
{
    private const string _name = "EUR";
    public Eur() : base(Eur._name)
    {

    }
}
