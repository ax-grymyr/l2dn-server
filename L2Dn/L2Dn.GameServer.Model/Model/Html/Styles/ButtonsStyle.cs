namespace L2Dn.GameServer.Model.Html.Styles;

public class ButtonsStyle: IHtmlStyle
{
    private const string DEFAULT_PAGE_LINK_FORMAT =
        "<td><button action=\"{0}\" value=\"{1}\" width=\"{2}\" height=\"{3}\" back=\"{4}\" fore=\"{5}\"></td>";

    private const string DEFAULT_PAGE_TEXT_FORMAT = "<td>{0}</td>";
    private const string DEFAULT_PAGER_SEPARATOR = "<td align=center> | </td>";

    public static readonly ButtonsStyle INSTANCE = new ButtonsStyle(40, 15, "L2UI_CT1.Button_DF", "L2UI_CT1.Button_DF");

    private readonly int _width;
    private readonly int _height;
    private readonly string _back;
    private readonly string _fore;

    public ButtonsStyle(int width, int height, string back, string fore)
    {
        _width = width;
        _height = height;
        _back = back;
        _fore = fore;
    }

    public string applyBypass(string bypass, string name, bool isEnabled)
    {
        if (isEnabled)
        {
            return string.Format(DEFAULT_PAGE_TEXT_FORMAT, name);
        }

        return string.Format(DEFAULT_PAGE_LINK_FORMAT, bypass, name, _width, _height, _back, _fore);
    }

    public string applySeparator()
    {
        return DEFAULT_PAGER_SEPARATOR;
    }
}