using System.Globalization;
using System.Text;
using Bank.Application.Exceptions;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Models.Factories.Currencies;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Services;

public class MyFinParser : IBankParser
{
    private readonly ILogger<MyFinParser> _logger;

    public MyFinParser(ILogger<MyFinParser> logger)
    {
        _logger = logger;
    }

    /// <exception cref="HtmlParseException"></exception>
    public List<Domain.Entities.Bank> Parse(HtmlDocument document, City city)
    {
        _logger.LogTrace("Parsing 'MyFin'");
        var bankList = new List<Domain.Entities.Bank>();
        var bank = new Domain.Entities.Bank();

        // Get table banks
        const string banksTableXPath = "//*[@id=\"currency-table\"]/tbody/tr";
        var banksNode = document.DocumentNode.SelectNodes(banksTableXPath)
                        ?? throw new HtmlParseNotFoundException(banksTableXPath);

        string bankId = null!;

        foreach (var bankNode in banksNode)
        {
            const string bankAttribute = "class";
            var classType = bankNode.Attributes[bankAttribute]?.Value
                            ?? throw new HtmlParseNotFoundException(bankNode.XPath, bankAttribute);

            // Parse information: 
            // 0 -> Name Bank / Street
            // 1 -> USD Buy
            // 2 -> USD Sell
            // 3 -> EUR Buy
            // 4 -> EUR Sell
            // 5 -> RUB Buy
            // 6 -> RUB Sell
            // 7 -> EUR->USD Buy
            // 8 -> EUR->USD Sell

            if (classType.Contains("static c-currency-table__main-row"))
            {
                ParseInternetBank(bankNode, bankList);
            }
            else if (classType.Contains("c-currency-table__main-row"))
            {
                ParseBank(bankNode, bank, ref bankId);
            }
            else if (classType.Contains("c-currency-table__additional-row"))
            {
                ParseDepartments(bankNode, bankList, bank, city, bankId);
                bank = new Domain.Entities.Bank();
            }
        }

        return bankList.Count < 1
            ? throw new HtmlParseException("Bank not found")
            : bankList;

        static void ParseInternetBank(HtmlNode bankNode, List<Domain.Entities.Bank> bankList)
        {
            // Internet bank

            var internetBank = new Domain.Entities.Bank();

            var information = new string[9];
            var iteration = 0;

            const string internetBankXPath = "./td";
            var bankNodeCollections = bankNode.SelectNodes(internetBankXPath)
                                      ?? throw new HtmlParseNotFoundException(bankNode.XPath + internetBankXPath);

            foreach (var informationNode in bankNodeCollections)
            {
                information[iteration++] = informationNode.InnerText;
            }

            internetBank.FullName = information[0].Trim();
            if (string.IsNullOrWhiteSpace(internetBank.FullName))
            {
                const string imageXPath = "./td[1]/span/span/img";
                var node = bankNode.SelectSingleNode(imageXPath)
                           ?? throw new HtmlParseNotFoundException(bankNode.XPath + imageXPath);

                const string imageAttribute = "data-url-img";
                var bankName = node.Attributes[imageAttribute]
                                   .Value
                                   .Split('/')[^1]
                                   .Split('.')[0]
                               ?? throw new HtmlParseNotFoundException(node.XPath, imageAttribute);

                // Internet bank: nembo_2.svg
                if (bankName.Contains('_'))
                {
                    bankName = bankName.Split('_')[0];
                }

                StringBuilder sb = new(bankName);
                sb[0] = char.ToUpper(sb[0]);

                internetBank.FullName = sb.ToString();
            }

            Currency?[] currencies =
            {
                Convert<Usd>(information[1], information[2]),
                Convert<Eur>(information[3], information[4]),
                Convert<Rub>(information[5], information[6]),
                Convert<EurToUsd>(information[7], information[8])
            };

            foreach (var currency in currencies)
            {
                if (currency is null)
                {
                    continue;
                }

                internetBank.BestCurrencies.Add(currency);
            }

            bankList.Add(internetBank);
        }

        static void ParseBank(HtmlNode bankNode, Domain.Entities.Bank bank, ref string bankId)
        {
            // Bank

            const string bankIdAttribute = "data-currencies-courses-bank-id";
            bankId = bankNode.Attributes[bankIdAttribute]?.Value
                     ?? throw new HtmlParseNotFoundException(bankNode.XPath, bankIdAttribute);

            var information = new string[9];
            var iteration = 0;

            const string bankXPath = "./td";
            var bankNodeCollections = bankNode.SelectNodes(bankXPath)
                                      ?? throw new HtmlParseNotFoundException(bankNode.XPath + bankXPath);

            foreach (var informationNode in bankNodeCollections)
            {
                information[iteration++] = informationNode.InnerText;
            }

            bank.FullName = information[0].Trim();

            if (string.IsNullOrEmpty(bank.FullName))
            {
                throw new HtmlParseException("Not found bank name");
            }

            Currency?[] currencies =
            {
                Convert<Usd>(information[1], information[2]),
                Convert<Eur>(information[3], information[4]),
                Convert<Rub>(information[5], information[6]),
                Convert<EurToUsd>(information[7], information[8])
            };

            foreach (var currency in currencies)
            {
                if (currency is null)
                {
                    continue;
                }

                bank.BestCurrencies.Add(currency);
            }
        }

        static void ParseDepartments(
            HtmlNode bankNode,
            List<Domain.Entities.Bank> bankList,
            Domain.Entities.Bank bank,
            City city,
            string bankId)
        {
            // Departments

            const string departmentIdAttribute = "data-currencies-courses-bank-id";
            var departmentsId = bankNode.Attributes[departmentIdAttribute]?.Value
                                ?? throw new HtmlParseNotFoundException(bankNode.XPath, departmentIdAttribute);

            if (bankId != departmentsId)
            {
                throw new HtmlParseException($"BankId = '{bankId}' not equals DepartmentsId = '{departmentsId}'");
            }

            const string departmentsXPath = "./td/table/tbody/tr";
            var departmentsNodeCollections = bankNode.SelectNodes(departmentsXPath)
                                             ?? throw new HtmlParseNotFoundException(bankNode.XPath + departmentsXPath);

            foreach (var departmentNode in departmentsNodeCollections)
            {
                // Get currencies information

                const string currenciesXPath = "./td";
                var currenciesInformation = departmentNode.SelectNodes(currenciesXPath)
                                            ?? throw new HtmlParseNotFoundException(departmentNode.XPath +
                                                currenciesXPath);

                var iteration = 0;
                var information = new string[9];

                foreach (var informationNode in currenciesInformation)
                {
                    information[iteration++] = informationNode.InnerText;
                }

                var street = information[0].Trim();
                var endIndex = street.LastIndexOf("  ", StringComparison.Ordinal);

                if (endIndex > 0)
                {
                    street = street.Substring(0, endIndex);
                }

                while (street.Contains("  "))
                {
                    street = street.Replace("  ", " ");
                }

                street = street.Replace('«', '\"').Replace('»', '\"');

                information[0] = street;

                Department department = new() { City = city };
                department.Street = information[0];

                Currency?[] currencies =
                {
                    Convert<Usd>(information[1], information[2]),
                    Convert<Eur>(information[3], information[4]),
                    Convert<Rub>(information[5], information[6]),
                    Convert<EurToUsd>(information[7], information[8])
                };

                foreach (var currency in currencies)
                {
                    if (currency is null)
                    {
                        continue;
                    }

                    department.Currencies.Add(currency);
                }

                bank.Departments.Add(department);
            }

            for (var i = 0; i < bank.Departments.Count - 1; i++)
            {
                for (var j = i + 1; j < bank.Departments.Count; j++)
                {
                    if (bank.Departments[i].Street == bank.Departments[j].Street)
                    {
                        bank.Departments.RemoveAt(j);
                        j--;
                    }
                }
            }

            bankList.Add(bank);
        }
    }

    private static Currency? Convert<T>(string? buy, string? sell) where T : Currency, new()
    {
        if (buy is null || sell is null)
        {
            return null;
        }

        IFormatProvider provider = new NumberFormatInfo
        {
            CurrencyDecimalSeparator = buy.Contains('.') ? "." : ","
        };

        if (!TryParse(buy, out var buyDec, provider) || !TryParse(sell, out var sellDec, provider))
        {
            return null;
        }

        Currency currency = new T
        {
            Buy = buyDec,
            Sell = sellDec
        };

        return currency;

        static bool TryParse(string text, out decimal value, IFormatProvider provider)
        {
            return decimal.TryParse(text, NumberStyles.Currency, provider, out value);
        }
    }
}