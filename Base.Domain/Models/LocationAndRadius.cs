using Base.Domain.Entities;

namespace Base.Domain.Models;

public class LocationAndRadius
{
    public Location Location { get; set; } = null!;
    public double Radius { get; set; }
}