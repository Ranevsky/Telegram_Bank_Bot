using Bank.Application.Exceptions;
using Bank.Application.Interfaces;
using Bank.Application.Models;
using Bank.Domain.Entities;
using Bank.Infrastructure.Services.HtmlDocuments;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Services;

public class GetBanksFromMyFin : IGetBanks
{
    private readonly IBankContext _db;
    private readonly IGetHtmlDocument _getterHtml;
    private readonly IBankParser _parser;

    public GetBanksFromMyFin(IBankContext db, MyFinGetterHtmlDocument myFinGetterHtml, MyFinParser parser)
    {
        _db = db;
        _getterHtml = myFinGetterHtml;
        _parser = parser;
    }

    public async Task<BanksWithInternetBanks> GetBanksAsync(City city)
    {
        try
        {
            _getterHtml.Object = city.Name;
            var document = _getterHtml.Document;

            var currencies = await _db.Currencies.ToArrayAsync();

            var banks = _parser.Parse(document, city, currencies);

            return banks;
        }
        catch (HtmlParseRedirectException ex)
        {
            throw new CityNotSupportedException(city, ex);
        }
        catch (CityNotValidException ex)
        {
            throw new CityNotSupportedException(city, ex);
        }
    }
}