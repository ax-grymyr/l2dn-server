namespace L2Dn.GameServer.Model.Html.Formatters;

public class DefaultFormatter: IBypassFormatter
{
    public static readonly DefaultFormatter INSTANCE = new DefaultFormatter();

    public String formatBypass(String bypass, int page)
    {
        return bypass + " " + page;
    }
}