using System.ComponentModel.DataAnnotations;

namespace Geocoding.Infrastructure.Models;

public class GoogleConfiguration
{
    [Required]
    public string Language { get; set; } = null!;

    [Required]
    public string Key { get; set; } = null!;
}