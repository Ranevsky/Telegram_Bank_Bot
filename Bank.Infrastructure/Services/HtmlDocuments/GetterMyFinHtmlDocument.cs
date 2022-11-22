using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Services.HtmlDocuments;

public class GetterMyFinHtmlDocument : GetterHtmlDocFromUrl
{
    private new const string Url = "https://myfin.by";
    private readonly ILogger<GetterMyFinHtmlDocument> _logger;
    private string? _object;

    public GetterMyFinHtmlDocument(ILogger<GetterMyFinHtmlDocument> logger)
        : base(Url)
    {
        _logger = logger;
    }

    public override string? Object
    {
        get => _object;
        set => _object = string.IsNullOrWhiteSpace(value)
            ? null
            : $"/currency/{value}";
    }

    public override HtmlDocument Document
    {
        get
        {
            _logger.LogInformation("Load website 'MyFin'");
            return base.Document;
        }
    }
}