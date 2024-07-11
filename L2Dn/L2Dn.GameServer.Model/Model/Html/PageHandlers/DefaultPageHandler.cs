using System.Globalization;
using System.Text;

namespace L2Dn.GameServer.Model.Html.PageHandlers;

public class DefaultPageHandler: IPageHandler
{
    public static readonly DefaultPageHandler INSTANCE = new DefaultPageHandler(2);
    protected readonly int _pagesOffset;

    public DefaultPageHandler(int pagesOffset)
    {
        _pagesOffset = pagesOffset;
    }

    public void apply(string bypass, int currentPage, int pages, StringBuilder sb, IBypassFormatter bypassFormatter,
        IHtmlStyle style)
    {
        int pagerStart = Math.Max(currentPage - _pagesOffset, 0);
        int pagerFinish = Math.Min(currentPage + _pagesOffset + 1, pages);

        // Show the initial pages in case we are in the middle or at the end
        if (pagerStart > _pagesOffset)
        {
            for (int i = 0; i < _pagesOffset; i++)
            {
                sb.Append(style.applyBypass(bypassFormatter.formatBypass(bypass, i),
                    (i + 1).ToString(CultureInfo.InvariantCulture),
                    currentPage == i));
            }

            // Separator
            sb.Append(style.applySeparator());
        }

        // Show current pages
        for (int i = pagerStart; i < pagerFinish; i++)
        {
            sb.Append(style.applyBypass(bypassFormatter.formatBypass(bypass, i),
                (i + 1).ToString(CultureInfo.InvariantCulture),
                currentPage == i));
        }

        // Show the last pages
        if (pages > pagerFinish)
        {
            // Separator
            sb.Append(style.applySeparator());

            for (int i = pages - _pagesOffset; i < pages; i++)
            {
                sb.Append(style.applyBypass(bypassFormatter.formatBypass(bypass, i),
                    (i + 1).ToString(CultureInfo.InvariantCulture),
                    currentPage == i));
            }
        }
    }
}