using Base.Domain.Entities;

namespace Bank.Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public double? Radius { get; set; }
    public Location? Location { get; set; }
    public DateTime LastUpdate { get; set; }
}