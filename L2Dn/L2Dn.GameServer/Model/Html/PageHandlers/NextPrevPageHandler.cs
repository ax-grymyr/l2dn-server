using System.Text;

namespace L2Dn.GameServer.Model.Html.PageHandlers;

/**
 * Creates pager with links << | < | > | >>
 * @author UnAfraid
 */
public class NextPrevPageHandler: IPageHandler
{
	public static readonly NextPrevPageHandler INSTANCE = new NextPrevPageHandler();

	public void apply(String bypass, int currentPage, int pages, StringBuilder sb, IBypassFormatter bypassFormatter,
		IHtmlStyle style)
	{
		// Beginning
		sb.Append(style.applyBypass(bypassFormatter.formatBypass(bypass, 0), "<<", (currentPage - 1) < 0));

		// Separator
		sb.Append(style.applySeparator());

		// Previous
		sb.Append(style.applyBypass(bypassFormatter.formatBypass(bypass, currentPage - 1), "<", currentPage <= 0));
		sb.Append(style.applySeparator());
		sb.Append($"<td align=\"center\">Page: {currentPage + 1}/{pages + 1}</td>");
		sb.Append(style.applySeparator());

		// Next
		sb.Append(style.applyBypass(bypassFormatter.formatBypass(bypass, currentPage + 1), ">", currentPage >= pages));

		// Separator
		sb.Append(style.applySeparator());

		// End
		sb.Append(style.applyBypass(bypassFormatter.formatBypass(bypass, pages), ">>", (currentPage + 1) > pages));
	}
}