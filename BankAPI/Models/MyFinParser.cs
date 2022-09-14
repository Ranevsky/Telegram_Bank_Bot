using System.Text;

using BankAPI.Exceptions;
using BankAPI.Models.Currencies;
using BankAPI.Models.Interfaces;

using HtmlAgilityPack;

namespace BankAPI.Models;

public class MyFinParser : IBankParser
{
    private static Currency? Convert<T>(string buy, string sell) where T : Currency, new()
    {
        if (buy == null && sell == null)
        {
            return null;
        }

        IFormatProvider provider = new System.Globalization.NumberFormatInfo()
        {
            CurrencyDecimalSeparator = buy!.Contains('.') ? "." : ","
        };

        if (!TryParse(buy, out decimal buyDec, provider) || !TryParse(sell, out decimal sellDec, provider))
        {
            return null;
        }

        Currency currency = new T()
        {
            Buy = buyDec,
            Sell = sellDec
        };

        return currency;

        static bool TryParse(string text, out decimal value, IFormatProvider provider)
        {
            return decimal.TryParse(text, System.Globalization.NumberStyles.Currency, provider, out value);
        }
    }

    /// <exception cref="HtmlParseException"></exception>
    public List<Bank> Parse(HtmlDocument document, City city)
    {
        List<Bank> bankList = new();
        Bank bank = new();

        // Get table banks
        const string banksTableXPath = "//*[@id=\"currency-table\"]/tbody/tr";
        HtmlNodeCollection banksNode = document.DocumentNode.SelectNodes(banksTableXPath)
            ?? throw new NotFoundHtmlParseException(banksTableXPath);

        string bankId = null!;

        foreach (HtmlNode bankNode in banksNode)
        {
            const string bankAttribute = "class";
            string classType = bankNode.Attributes[bankAttribute]?.Value
                ?? throw new NotFoundHtmlParseException(bankNode.XPath, bankAttribute);

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
                bank = new();
            }
        }
        return bankList.Count < 1
            ? throw new HtmlParseException("Bank not found")
            : bankList;

        static void ParseInternetBank(HtmlNode bankNode, List<Bank> bankList)
        {
            // Internet bank

            Bank internetBank = new();

            string[] information = new string[9];
            int iteration = 0;

            const string internetBankXPath = "./td";
            HtmlNodeCollection bankNodeCollections = bankNode.SelectNodes(internetBankXPath)
                ?? throw new NotFoundHtmlParseException(bankNode.XPath + internetBankXPath);

            foreach (HtmlNode informationNode in bankNodeCollections)
            {
                information[iteration++] = informationNode.InnerText;
            }

            internetBank.FullName = information[0].Trim();
            if (string.IsNullOrWhiteSpace(internetBank.FullName))
            {
                const string imageXPath = "./td[1]/span/span/img";
                HtmlNode node = bankNode.SelectSingleNode(imageXPath)
                    ?? throw new NotFoundHtmlParseException(bankNode.XPath + imageXPath);

                const string imageAttribute = "data-url-img";
                string bankName = node.Attributes[imageAttribute]
                    .Value
                    .Split('/')[^1]
                    .Split('.')[0]
                    ?? throw new NotFoundHtmlParseException(node.XPath, imageAttribute);

                // Internet bank: nembo_2.svg
                if (bankName.Contains('_'))
                {
                    bankName = bankName.Split('_')[0];
                }

                StringBuilder sb = new(bankName);
                sb[0] = char.ToUpper(sb[0]);

                internetBank.FullName = sb.ToString();
            }

            Currency?[] currencies = new[]
            {
                Convert<Usd>(information[1], information[2]),
                Convert<Eur>(information[3], information[4]),
                Convert<Rub>(information[5], information[6]),
                Convert<EurToUsd>(information[7], information[8])
            };

            foreach (Currency? currency in currencies)
            {
                if (currency == null)
                {
                    continue;
                }
                internetBank.BestCurrencies.Add(currency);
            }
            bankList.Add(internetBank);
        }
        static void ParseBank(HtmlNode bankNode, Bank bank, ref string bankId)
        {
            // Bank

            const string bankIdAttribute = "data-currencies-courses-bank-id";
            bankId = bankNode.Attributes[bankIdAttribute]?.Value
                ?? throw new NotFoundHtmlParseException(bankNode.XPath, bankIdAttribute);

            string[] information = new string[9];
            int iteration = 0;

            const string bankXPath = "./td";
            HtmlNodeCollection bankNodeCollections = bankNode.SelectNodes(bankXPath)
                ?? throw new NotFoundHtmlParseException(bankNode.XPath + bankXPath);

            foreach (HtmlNode informationNode in bankNodeCollections)
            {
                information[iteration++] = informationNode.InnerText;
            }

            bank.FullName = information[0].Trim();

            if (string.IsNullOrEmpty(bank.FullName))
            {
                throw new HtmlParseException("Not found bank name");
            }

            Currency?[] currencies = new[]
            {
                Convert<Usd>(information[1], information[2]),
                Convert<Eur>(information[3], information[4]),
                Convert<Rub>(information[5], information[6]),
                Convert<EurToUsd>(information[7], information[8])
            };

            foreach (Currency? currency in currencies)
            {
                if (currency == null)
                {
                    continue;
                }
                bank.BestCurrencies.Add(currency);
            }
        }
        static void ParseDepartments(HtmlNode bankNode, List<Bank> bankList, Bank bank, City city, string bankId)
        {
            // Deparments

            const string departmentIdAttribute = "data-currencies-courses-bank-id";
            string departmentsId = bankNode.Attributes[departmentIdAttribute]?.Value
                ?? throw new NotFoundHtmlParseException(bankNode.XPath, departmentIdAttribute);

            if (bankId != departmentsId)
            {
                throw new HtmlParseException($"BankId = '{bankId}' not equals DepartmentsId = '{departmentsId}'");
            }

            const string departmentsXPath = "./td/table/tbody/tr";
            HtmlNodeCollection departmentsNodeCollections = bankNode.SelectNodes(departmentsXPath)
                ?? throw new NotFoundHtmlParseException(bankNode.XPath + departmentsXPath);

            foreach (HtmlNode departmentNode in departmentsNodeCollections)
            {
                // Get currencies information

                const string currenciesXPath = "./tdg";
                HtmlNodeCollection currenciesInformation = departmentNode.SelectNodes(currenciesXPath)
                    ?? throw new NotFoundHtmlParseException(departmentNode.XPath + currenciesXPath);

                int iteration = 0;
                string[] information = new string[9];

                foreach (HtmlNode informationNode in currenciesInformation)
                {
                    information[iteration++] = informationNode.InnerText;
                }

                string street = information[0].Trim();
                int endIndex = street.LastIndexOf("  ");

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

                Currency?[] currencies = new[]
                {
                    Convert<Usd>(information[1], information[2]),
                    Convert<Eur>(information[3], information[4]),
                    Convert<Rub>(information[5], information[6]),
                    Convert<EurToUsd>(information[7], information[8])
                };

                foreach (Currency? currency in currencies)
                {
                    if (currency == null)
                    {
                        continue;
                    }
                    department.Currencies.Add(currency);
                }
                bank.Departments.Add(department);
            }

            for (int i = 0; i < bank.Departments.Count - 1; i++)
            {
                for (int j = i + 1; j < bank.Departments.Count; j++)
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
}
