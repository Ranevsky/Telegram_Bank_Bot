namespace GeocodingAPI.Models;

public class LocationAndRadius
{
    public Location Location { get; set; } = null!;
    public double Radius { get; set; }
}
