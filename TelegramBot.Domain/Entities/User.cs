using Bank.Domain.Entities;
using Base.Domain.Entities;

namespace TelegramBot.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? LanguageCode { get; set; }
    public bool IsBot { get; set; }
    public bool IsActive { get; set; } = true;
    public Currency? SelectedCurrency { get; set; }
    public bool? IsBuyOperation { get; set; }
    public Location? Location { get; set; }
    public City? NearCity { get; set; }
}