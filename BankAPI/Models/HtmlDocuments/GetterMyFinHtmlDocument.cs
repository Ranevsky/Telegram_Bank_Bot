namespace BankAPI.Models.HtmlDocuments;

public class GetterMyFinHtmlDocument : GetterHtmlDocFromUrl
{
    private const string URL = "https://myfin.by";

    public override string? Object
    {
        get => _object;
        set => _object = string.IsNullOrWhiteSpace(value)
                ? null
                : $"/currency/{value}";
    }
    private string? _object;

    public GetterMyFinHtmlDocument()
        : base(URL)
    {

    }
}
