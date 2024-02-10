using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author Nik
 */
public class OnCreatureTeleport: IBaseEvent
{
	private readonly Creature _creature;
	private readonly int _destX;
	private readonly int _destY;
	private readonly int _destZ;
	private readonly int _destHeading;
	private readonly Instance _destInstance;
	
	public OnCreatureTeleport(Creature creature, int destX, int destY, int destZ, int destHeading, Instance destInstance)
	{
		_creature = creature;
		_destX = destX;
		_destY = destY;
		_destZ = destZ;
		_destHeading = destHeading;
		_destInstance = destInstance;
	}
	
	public Creature getCreature()
	{
		return _creature;
	}
	
	public int getDestX()
	{
		return _destX;
	}
	
	public int getDestY()
	{
		return _destY;
	}
	
	public int getDestZ()
	{
		return _destZ;
	}
	
	public int getDestHeading()
	{
		return _destHeading;
	}
	
	public Instance getDestInstance()
	{
		return _destInstance;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_TELEPORT;
	}
}