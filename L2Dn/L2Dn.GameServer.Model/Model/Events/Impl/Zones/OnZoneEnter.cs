using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Model.Events.Impl.Zones;

/**
 * @author UnAfraid
 */
public class OnZoneEnter: EventBase
{
	private readonly Creature _creature;
	private readonly Zone _zone;
	
	public OnZoneEnter(Creature creature, Zone zone)
	{
		_creature = creature;
		_zone = zone;
	}
	
	public Creature getCreature()
	{
		return _creature;
	}
	
	public Zone getZone()
	{
		return _zone;
	}
}