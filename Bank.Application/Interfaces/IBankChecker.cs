﻿using Bank.Application.Models;
using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IBankChecker
{
    Task CheckAsync(BanksWithInternetBanks newBanks, City city);
}