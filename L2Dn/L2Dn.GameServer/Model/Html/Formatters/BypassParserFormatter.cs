namespace L2Dn.GameServer.Model.Html.Formatters;

public class BypassParserFormatter: IBypassFormatter
{
    public static readonly BypassParserFormatter INSTANCE = new BypassParserFormatter();

    public String formatBypass(String bypass, int page)
    {
        return bypass + " page=" + page;
    }
}