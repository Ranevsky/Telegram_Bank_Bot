using System.Text.Json.Serialization;
using Base.Domain.Entities;

namespace Bank.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Street { get; set; } = null!;
    public Location? Location { get; set; }
    public List<CurrencyExchange> Currencies { get; set; } = new();
    public City City { get; set; } = null!;

    [JsonIgnore]
    public Bank Bank { get; set; } = null!;
}