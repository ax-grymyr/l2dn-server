namespace L2Dn.GameServer.Model.Html.Formatters;

public class BypassParserFormatter: IBypassFormatter
{
    public static readonly BypassParserFormatter INSTANCE = new BypassParserFormatter();

    public string formatBypass(string bypass, int page)
    {
        return bypass + " page=" + page;
    }
}