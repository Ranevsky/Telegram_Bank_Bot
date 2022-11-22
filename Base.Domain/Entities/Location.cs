using System.Text.Json.Serialization;

namespace Base.Domain.Entities;

public class Location
{
    public int Id { get; set; }
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    // Todo: jsonIgnore to notmapped or ignore
    [JsonIgnore]
    public long? UserId { get; set; }

    [JsonIgnore]
    public int? CityId { get; set; }

    public static double Distance(Location location1, Location location2)
    {
        var p = Math.PI / 180;
        var diff = 0.5 - Math.Cos((location2.Latitude - location1.Latitude) * p) / 2 +
                   Math.Cos(location1.Latitude * p) * Math.Cos(location2.Latitude * p) *
                   (1 - Math.Cos((location2.Longitude - location1.Longitude) * p)) / 2;
        return 12742 * Math.Asin(Math.Sqrt(diff));
    }
}