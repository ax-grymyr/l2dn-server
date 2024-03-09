using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Scripts.Handlers.TargetHandlers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class TargetHandler: IHandler<ITargetTypeHandler, TargetType>
{
	private readonly Map<TargetType, ITargetTypeHandler> _datatable;
	
	protected TargetHandler()
	{
		_datatable = new();
		registerHandler(new AdvanceBase());
		registerHandler(new Artillery());
		registerHandler(new DoorTreasure());
		registerHandler(new Enemy());
		registerHandler(new EnemyNot());
		registerHandler(new EnemyOnly());
		registerHandler(new FortressFlagpole());
		registerHandler(new Ground());
		registerHandler(new HolyThing());
		registerHandler(new Item());
		registerHandler(new MyMentor());
		registerHandler(new MyParty());
		registerHandler(new None());
		registerHandler(new NpcBody());
		registerHandler(new Others());
		registerHandler(new OwnerPet());
		registerHandler(new PcBody());
		registerHandler(new Pet());
		registerHandler(new Self());
		registerHandler(new Summon());
		registerHandler(new Target());
		registerHandler(new WyvernTarget());
	}
	
	public void registerHandler(ITargetTypeHandler handler)
	{
		_datatable.put(handler.getTargetType(), handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(ITargetTypeHandler handler)
	{
		_datatable.remove(handler.getTargetType());
	}
	
	public ITargetTypeHandler getHandler(TargetType targetType)
	{
		return _datatable.get(targetType);
	}
	
	public int size()
	{
		return _datatable.size();
	}
	
	public static TargetHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly TargetHandler INSTANCE = new TargetHandler();
	}
}