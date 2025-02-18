using System.Runtime.CompilerServices;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class UserCommandHandler: IHandler<IUserCommandHandler, int>
{
	private readonly Map<int, IUserCommandHandler> _datatable;

	protected UserCommandHandler()
	{
		_datatable = new();
	}

	public void registerHandler(IUserCommandHandler handler)
	{
		foreach (int id in handler.getUserCommandList())
		{
			_datatable.put(id, handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IUserCommandHandler handler)
	{
		foreach (int id in handler.getUserCommandList())
		{
			_datatable.remove(id);
		}
	}

	public IUserCommandHandler? getHandler(int userCommand)
	{
		return _datatable.get(userCommand);
	}

	public int size()
	{
		return _datatable.Count;
	}

	public static UserCommandHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly UserCommandHandler INSTANCE = new();
	}
}