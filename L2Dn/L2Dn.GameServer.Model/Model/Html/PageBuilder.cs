using System.Text;
using L2Dn.GameServer.Model.Html.Formatters;
using L2Dn.GameServer.Model.Html.PageHandlers;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Html;

public static class PageBuilder
{
	public static PageBuilder<T> newBuilder<T>(List<T> elements, int elementsPerPage, string bypass)
	{
		return new PageBuilder<T>(elements, elementsPerPage, bypass.Trim());
	}
	
	public static PageBuilder<T> newBuilder<T>(T[] elements, int elementsPerPage, string bypass)
	{
		return new PageBuilder<T>(elements.ToList(), elementsPerPage, bypass.Trim());
	}
}

public class PageBuilder<T>
{
	private readonly List<T> _elements;
	private readonly int _elementsPerPage;
	private readonly string _bypass;
	private int _currentPage = 0;
	private IPageHandler _pageHandler = DefaultPageHandler.INSTANCE;
	private IBypassFormatter _formatter = DefaultFormatter.INSTANCE;
	private IHtmlStyle _style = DefaultStyle.INSTANCE;
	private Action<int, T, StringBuilder> _bodyHandler;
	
	public PageBuilder(List<T> elements, int elementsPerPage, string bypass)
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
	
	public PageBuilder<T> bodyHandler(Action<int, T, StringBuilder> bodyHandler)
	{
		ArgumentNullException.ThrowIfNull(bodyHandler);
		_bodyHandler = bodyHandler;
		return this;
	}
	
	public PageBuilder<T> bodyHandler(IBodyHandler<T> bodyHandler)
	{
		ArgumentNullException.ThrowIfNull(bodyHandler);
		_bodyHandler = bodyHandler.apply;
		return this;
	}

	
	public PageBuilder<T> pageHandler(IPageHandler pageHandler)
	{
		ArgumentNullException.ThrowIfNull(pageHandler);
		_pageHandler = pageHandler;
		return this;
	}
	
	public PageBuilder<T> formatter(IBypassFormatter formatter)
	{
		ArgumentNullException.ThrowIfNull(formatter);
		_formatter = formatter;
		return this;
	}
	
	public PageBuilder<T> style(IHtmlStyle style)
	{
		ArgumentNullException.ThrowIfNull(style);
		_style = style;
		return this;
	}
	
	public PageResult build()
	{
		if (_bodyHandler is null)
			throw new InvalidOperationException("Body was not set!");

		int pages = _elements.Count / _elementsPerPage + (_elements.Count % _elementsPerPage > 0 ? 1 : 0);
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
		create(_bodyHandler, _elements, pages, start, _elementsPerPage, sb);
		return new PageResult(pages, pagerTemplate, sb);
	}
	
	private static void create(Action<int, T, StringBuilder> apply, IEnumerable<T> elements, int pages, int start, int elementsPerPage, StringBuilder sb)
	{
		int i = 0;
		foreach (T element in elements)
		{
			if (i++ < start)
			{
				continue;
			}

			apply(pages, element, sb);

			if (i >= elementsPerPage + start)
			{
				break;
			}
		}
	}
}