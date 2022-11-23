using Bank.Domain.Entities;

namespace Bank.Application.Models;

public class DepartmentByDistance
{
    public double Distance { get; set; }
    public Department Department { get; set; } = null!;
    public CurrencyExchange CurrencyExchange { get; set; } = null!;
}