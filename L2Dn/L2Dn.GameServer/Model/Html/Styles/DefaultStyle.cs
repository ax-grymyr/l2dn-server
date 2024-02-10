namespace L2Dn.GameServer.Model.Html.Styles;

public class DefaultStyle: IHtmlStyle
{
    private const String DEFAULT_PAGE_LINK_FORMAT = "<td><a action=\"{0}\">{1}</a></td>";
    private const String DEFAULT_PAGE_TEXT_FORMAT = "<td>{0}</td>";
    private const String DEFAULT_PAGER_SEPARATOR = "<td align=center> | </td>";

    public static readonly DefaultStyle INSTANCE = new DefaultStyle();

    public String applyBypass(String bypass, String name, bool isEnabled)
    {
        if (isEnabled)
        {
            return String.Format(DEFAULT_PAGE_TEXT_FORMAT, name);
        }

        return String.Format(DEFAULT_PAGE_LINK_FORMAT, bypass, name);
    }

    public String applySeparator()
    {
        return DEFAULT_PAGER_SEPARATOR;
    }
}