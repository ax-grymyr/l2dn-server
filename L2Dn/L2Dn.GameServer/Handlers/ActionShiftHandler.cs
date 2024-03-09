using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Scripts.Handlers.ActionShiftHandlers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class ActionShiftHandler: IHandler<IActionShiftHandler, InstanceType>
{
	private readonly Map<InstanceType, IActionShiftHandler> _actionsShift;
	
	protected ActionShiftHandler()
	{
		_actionsShift = new();
		
		// Action Shift Handlers
		registerHandler(new DoorActionShift());
		registerHandler(new ItemActionShift());
		registerHandler(new NpcActionShift());
		registerHandler(new PlayerActionShift());
		registerHandler(new StaticObjectActionShift());
		registerHandler(new SummonActionShift());
	}
	
	public void registerHandler(IActionShiftHandler handler)
	{
		_actionsShift.put(handler.getInstanceType(), handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IActionShiftHandler handler)
	{
		_actionsShift.remove(handler.getInstanceType());
	}
	
	public IActionShiftHandler getHandler(InstanceType iType)
	{
		IActionShiftHandler result = null;
		for (InstanceType? t = iType; t != null; t = t.Value.GetParent())
		{
			result = _actionsShift.get(t.Value);
			if (result != null)
			{
				break;
			}
		}
		return result;
	}
	
	public int size()
	{
		return _actionsShift.size();
	}
	
	public static ActionShiftHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ActionShiftHandler INSTANCE = new ActionShiftHandler();
	}
}