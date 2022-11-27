using System.Text.Json.Serialization;

namespace Bank.Domain.Entities;

public abstract class BaseBank
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? ShortName { get; set; }

    [JsonIgnore]
    public string Name => ShortName ?? FullName;
}