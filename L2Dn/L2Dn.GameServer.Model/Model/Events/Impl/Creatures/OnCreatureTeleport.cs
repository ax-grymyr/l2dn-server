using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author Nik
 */
public class OnCreatureTeleport: LocationEventBase
{
	private readonly Creature _creature;
	private readonly LocationHeading _location;
	private readonly Instance _destInstance;
	
	public OnCreatureTeleport(Creature creature, LocationHeading location, Instance destInstance)
	{
		_creature = creature;
		_location = location;
		_destInstance = destInstance;
	}
	
	public Creature getCreature()
	{
		return _creature;
	}

	public LocationHeading Location => _location;

	public Instance getDestInstance()
	{
		return _destInstance;
	}
}