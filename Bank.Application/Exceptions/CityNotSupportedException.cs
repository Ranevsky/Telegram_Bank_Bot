using Bank.Domain.Entities;

namespace Bank.Application.Exceptions;

public class CityNotSupportedException : Exception
{
    public CityNotSupportedException(City city, Exception ex)
        : base(ex.Message, ex)
    {
        City = city;
    }

    public City City { get; }
}