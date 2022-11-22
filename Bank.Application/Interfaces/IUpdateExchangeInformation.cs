namespace Bank.Application.Interfaces;

public interface IUpdateExchangeInformation
{
    public Task UpdateAsync(string cityName, bool checkTime = true);
}