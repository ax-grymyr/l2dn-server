using System.Runtime.CompilerServices;
using L2Dn.GameServer.Handlers.PlayerActions;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class PlayerActionHandler: IHandler<IPlayerActionHandler, String>
{
	private readonly Map<String, IPlayerActionHandler> _actions = new();
	
	protected PlayerActionHandler()
	{
		registerHandler(new AirshipAction());
		registerHandler(new BotReport());
		registerHandler(new InstanceZoneInfo());
		registerHandler(new PetAttack());
		registerHandler(new PetHold());
		registerHandler(new PetMove());
		registerHandler(new PetSkillUse());
		registerHandler(new PetStop());
		registerHandler(new PrivateStore());
		registerHandler(new Ride());
		registerHandler(new RunWalk());
		registerHandler(new ServitorAttack());
		registerHandler(new ServitorHold());
		registerHandler(new ServitorMode());
		registerHandler(new ServitorMove());
		registerHandler(new ServitorSkillUse());
		registerHandler(new ServitorStop());
		registerHandler(new SitStand());
		registerHandler(new SocialAction());
		registerHandler(new TacticalSignTarget());
		registerHandler(new TacticalSignUse());
		registerHandler(new TeleportBookmark());
		registerHandler(new UnsummonPet());
		registerHandler(new UnsummonServitor());
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
	
	public IPlayerActionHandler getHandler(String name)
	{
		return _actions.get(name);
	}
	
	public int size()
	{
		return _actions.size();
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