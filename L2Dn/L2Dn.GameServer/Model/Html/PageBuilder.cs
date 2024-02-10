using System.Text;
using L2Dn.GameServer.Model.Html.Formatters;
using L2Dn.GameServer.Model.Html.PageHandlers;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Html;

public class PageBuilder<T>
{
	private readonly List<T> _elements;
	private readonly int _elementsPerPage;
	private readonly String _bypass;
	private int _currentPage = 0;
	private IPageHandler _pageHandler = DefaultPageHandler.INSTANCE;
	private IBypassFormatter _formatter = DefaultFormatter.INSTANCE;
	private IHtmlStyle _style = DefaultStyle.INSTANCE;
	private IBodyHandler<T> _bodyHandler;
	
	private PageBuilder(List<T> elements, int elementsPerPage, String bypass)
	{
		_elements = elements;
		_elementsPerPage = elementsPerPage;
		_bypass = bypass;
	}
	
	public PageBuilder<T> currentPage(int currentPage)
	{
		_currentPage = Math.Max(currentPage, 0);
		return this;
	}
	
	public PageBuilder<T> bodyHandler(IBodyHandler<T> bodyHandler)
	{
		Objects.requireNonNull(bodyHandler, "Body Handler cannot be null!");
		_bodyHandler = bodyHandler;
		return this;
	}
	
	public PageBuilder<T> pageHandler(IPageHandler pageHandler)
	{
		Objects.requireNonNull(pageHandler, "Page Handler cannot be null!");
		_pageHandler = pageHandler;
		return this;
	}
	
	public PageBuilder<T> formatter(IBypassFormatter formatter)
	{
		Objects.requireNonNull(formatter, "Formatter cannot be null!");
		_formatter = formatter;
		return this;
	}
	
	public PageBuilder<T> style(IHtmlStyle style)
	{
		Objects.requireNonNull(style, "Style cannot be null!");
		_style = style;
		return this;
	}
	
	public PageResult build()
	{
		Objects.requireNonNull(_bodyHandler, "Body was not set!");
		
		int pages = (_elements.Count / _elementsPerPage) + ((_elements.Count % _elementsPerPage) > 0 ? 1 : 0);
		StringBuilder pagerTemplate = new StringBuilder();
		if (pages > 1)
		{
			_pageHandler.apply(_bypass, _currentPage, pages, pagerTemplate, _formatter, _style);
		}
		
		if (_currentPage > pages)
		{
			_currentPage = pages - 1;
		}
		
		int start = Math.Max(_elementsPerPage * _currentPage, 0);
		StringBuilder sb = new StringBuilder();
		_bodyHandler.create(_elements, pages, start, _elementsPerPage, sb);
		return new PageResult(pages, pagerTemplate, sb);
	}
	
	public static PageBuilder<T> newBuilder(List<T> elements, int elementsPerPage, String bypass)
	{
		return new PageBuilder<T>(elements, elementsPerPage, bypass.Trim());
	}
	
	public static PageBuilder<T> newBuilder(T[] elements, int elementsPerPage, String bypass)
	{
		return new PageBuilder<T>(elements.ToList(), elementsPerPage, bypass.Trim());
	}
}