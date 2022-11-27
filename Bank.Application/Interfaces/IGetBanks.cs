using Bank.Application.Models;
using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IGetBanks
{
    Task<BanksWithInternetBanks> GetBanksAsync(City city);
}