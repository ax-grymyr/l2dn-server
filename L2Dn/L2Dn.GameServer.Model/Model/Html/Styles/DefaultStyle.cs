namespace L2Dn.GameServer.Model.Html.Styles;

public class DefaultStyle: IHtmlStyle
{
    private const string DEFAULT_PAGE_LINK_FORMAT = "<td><a action=\"{0}\">{1}</a></td>";
    private const string DEFAULT_PAGE_TEXT_FORMAT = "<td>{0}</td>";
    private const string DEFAULT_PAGER_SEPARATOR = "<td align=center> | </td>";

    public static readonly DefaultStyle INSTANCE = new DefaultStyle();

    public string applyBypass(string bypass, string name, bool isEnabled)
    {
        if (isEnabled)
        {
            return string.Format(DEFAULT_PAGE_TEXT_FORMAT, name);
        }

        return string.Format(DEFAULT_PAGE_LINK_FORMAT, bypass, name);
    }

    public string applySeparator()
    {
        return DEFAULT_PAGER_SEPARATOR;
    }
}