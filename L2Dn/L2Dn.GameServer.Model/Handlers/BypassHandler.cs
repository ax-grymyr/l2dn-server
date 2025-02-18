using System.Runtime.CompilerServices;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author nBd, UnAfraid
 */
public class BypassHandler: IHandler<IBypassHandler, string>
{
	private readonly Map<string, IBypassHandler> _datatable;

	protected BypassHandler()
	{
		_datatable = new();
	}

	public void registerHandler(IBypassHandler handler)
	{
		foreach (string element in handler.getBypassList())
		{
			_datatable.put(element.ToLower(), handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IBypassHandler handler)
	{
		foreach (string element in handler.getBypassList())
		{
			_datatable.remove(element.ToLower());
		}
	}

	public IBypassHandler? getHandler(string commandValue)
	{
		string command = commandValue;
		if (command.Contains(" "))
		{
			command = command.Substring(0, command.IndexOf(' '));
		}
		return _datatable.get(command.ToLower());
	}

	public int size()
	{
		return _datatable.Count;
	}

	public static BypassHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly BypassHandler INSTANCE = new BypassHandler();
	}
}