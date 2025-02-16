using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class ActionHandler: IHandler<IActionHandler, InstanceType>
{
	private readonly Map<InstanceType, IActionHandler> _actions;
	
	public static ActionHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	protected ActionHandler()
	{
		_actions = new();
	}
	
	public void registerHandler(IActionHandler handler)
	{
		_actions.put(handler.getInstanceType(), handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IActionHandler handler)
	{
		_actions.remove(handler.getInstanceType());
	}
	
	public IActionHandler? getHandler(InstanceType iType)
	{
		IActionHandler? result = null;
		for (InstanceType? t = iType; t != null; t = t.Value.GetParent())
		{
			result = _actions.get(t.Value);
			if (result != null)
			{
				break;
			}
		}
		
		return result;
	}
	
	public int size()
	{
		return _actions.Count;
	}
	
	private static class SingletonHolder
	{
		public static readonly ActionHandler INSTANCE = new ActionHandler();
	}
}