using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Nik
 */
public class AffectObjectHandler: IHandler<IAffectObjectHandler, AffectObject>
{
	private readonly Map<AffectObject, IAffectObjectHandler> _datatable;
	
	protected AffectObjectHandler()
	{
		_datatable = new();
		
		registerHandler(new All());
		registerHandler(new Clan());
		registerHandler(new Friend());
		registerHandler(new FriendPc());
		registerHandler(new HiddenPlace());
		registerHandler(new Invisible());
		registerHandler(new NotFriend());
		registerHandler(new NotFriendPc());
		registerHandler(new ObjectDeadNpcBody());
		registerHandler(new UndeadRealEnemy());
		registerHandler(new WyvernObject());
	}
	
	public void registerHandler(IAffectObjectHandler handler)
	{
		_datatable.put(handler.getAffectObjectType(), handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IAffectObjectHandler handler)
	{
		_datatable.remove(handler.getAffectObjectType());
	}
	
	public IAffectObjectHandler getHandler(AffectObject targetType)
	{
		return _datatable.get(targetType);
	}
	
	public int size()
	{
		return _datatable.size();
	}
	
	public static AffectObjectHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AffectObjectHandler INSTANCE = new AffectObjectHandler();
	}
}