using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author UnAfraid
 */
public class OnZoneExited: IBaseEvent
{
	private readonly Creature _creature;
	private readonly ZoneType _zone;
	
	public OnZoneExited(Creature creature, ZoneType zone)
	{
		_creature = creature;
		_zone = zone;
	}
	
	public Creature getCreature()
	{
		return _creature;
	}
	
	public ZoneType getZone()
	{
		return _zone;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_ZONE_EXIT;
	}
}