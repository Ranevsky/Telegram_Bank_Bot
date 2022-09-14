namespace BankAPI.Models;

public class Location
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public static double Diff(Location location1, Location location2)
    {
        double p = Math.PI / 180;
        double diff = 0.5 - (Math.Cos((location2.Latitude - location1.Latitude) * p) / 2) + (Math.Cos(location1.Latitude * p) * Math.Cos(location2.Latitude * p) * (1 - Math.Cos((location2.Longitude - location1.Longitude) * p)) / 2);
        return 12742 * Math.Asin(Math.Sqrt(diff));
    }
    public static explicit operator Location(GeocodingAPI.Models.Location location)
    {
        Location loc = new()
        {
            Longitude = location.Longitude,
            Latitude = location.Latitude,
        };
        return loc;
    }
}
