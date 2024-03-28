using System.Runtime.CompilerServices;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author nBd, UnAfraid
 */
public class BypassHandler: IHandler<IBypassHandler, String>
{
	private readonly Map<String, IBypassHandler> _datatable;
	
	protected BypassHandler()
	{
		_datatable = new();
	}
	
	public void registerHandler(IBypassHandler handler)
	{
		foreach (String element in handler.getBypassList())
		{
			_datatable.put(element.ToLower(), handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IBypassHandler handler)
	{
		foreach (String element in handler.getBypassList())
		{
			_datatable.remove(element.ToLower());
		}
	}
	
	public IBypassHandler getHandler(String commandValue)
	{
		String command = commandValue;
		if (command.Contains(" "))
		{
			command = command.Substring(0, command.IndexOf(' '));
		}
		return _datatable.get(command.ToLower());
	}
	
	public int size()
	{
		return _datatable.size();
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