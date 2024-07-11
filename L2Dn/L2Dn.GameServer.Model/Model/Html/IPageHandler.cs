using System.Text;

namespace L2Dn.GameServer.Model.Html;

public interface IPageHandler
{
    void apply(string bypass, int currentPage, int pages, StringBuilder sb, IBypassFormatter bypassFormatter, IHtmlStyle style);
}