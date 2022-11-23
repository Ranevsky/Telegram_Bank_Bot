﻿using System.Text.Json.Serialization;

namespace Bank.Domain.Entities;

public class Bank
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? ShortName { get; set; }
    public List<CurrencyExchange> BestCurrencies { get; set; } = new();
    public List<Department> Departments { get; set; } = new();

    [JsonIgnore]
    public string Name => ShortName ?? FullName;
}