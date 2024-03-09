using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers.ActionHandlers;
using L2Dn.GameServer.Utilities;
using PlayerAction = L2Dn.GameServer.Enums.PlayerAction;
using TrapAction = L2Dn.GameServer.Enums.TrapAction;

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
		
		registerHandler(new ArtefactAction());
		registerHandler(new DecoyAction());
		registerHandler(new DoorAction());
		registerHandler(new ItemAction());
		registerHandler(new NpcAction());
		registerHandler(new ActionHandlers.PlayerAction());
		registerHandler(new PetAction());
		registerHandler(new StaticObjectAction());
		registerHandler(new SummonAction());
		registerHandler(new ActionHandlers.TrapAction());
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
	
	public IActionHandler getHandler(InstanceType iType)
	{
		IActionHandler result = null;
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
		return _actions.size();
	}
	
	private static class SingletonHolder
	{
		public static readonly ActionHandler INSTANCE = new ActionHandler();
	}
}