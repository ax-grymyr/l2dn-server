using System.Runtime.CompilerServices;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class PlayerActionHandler: IHandler<IPlayerActionHandler, string>
{
	private readonly Map<string, IPlayerActionHandler> _actions = new();

	protected PlayerActionHandler()
	{
	}

	public void registerHandler(IPlayerActionHandler handler)
	{
		_actions.put(handler.GetType().Name, handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IPlayerActionHandler handler)
	{
		_actions.remove(handler.GetType().Name);
	}

	public IPlayerActionHandler? getHandler(string name)
	{
		return _actions.get(name);
	}

	public int size()
	{
		return _actions.Count;
	}

	public static PlayerActionHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public  static readonly PlayerActionHandler INSTANCE = new PlayerActionHandler();
	}
}