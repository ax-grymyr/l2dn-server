namespace L2Dn.GameServer.Model.Html.Formatters;

public class DefaultFormatter: IBypassFormatter
{
    public static readonly DefaultFormatter INSTANCE = new DefaultFormatter();

    public string formatBypass(string bypass, int page)
    {
        return bypass + " " + page;
    }
}