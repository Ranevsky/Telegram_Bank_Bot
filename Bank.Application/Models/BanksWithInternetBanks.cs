using Bank.Domain.Entities;

namespace Bank.Application.Models;

public class BanksWithInternetBanks
{
    public List<Domain.Entities.Bank> Banks { get; set; } = new();
    public List<InternetBank> InternetBanks { get; set; } = new();
}