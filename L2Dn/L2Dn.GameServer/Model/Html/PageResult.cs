using System.Text;

namespace L2Dn.GameServer.Model.Html;

public class PageResult
{
    private readonly int _pages;
    private readonly StringBuilder _pagerTemplate;
    private readonly StringBuilder _bodyTemplate;
	
    public PageResult(int pages, StringBuilder pagerTemplate, StringBuilder bodyTemplate)
    {
        _pages = pages;
        _pagerTemplate = pagerTemplate;
        _bodyTemplate = bodyTemplate;
    }
	
    public int getPages()
    {
        return _pages;
    }
	
    public StringBuilder getPagerTemplate()
    {
        return _pagerTemplate;
    }
	
    public StringBuilder getBodyTemplate()
    {
        return _bodyTemplate;
    }
}