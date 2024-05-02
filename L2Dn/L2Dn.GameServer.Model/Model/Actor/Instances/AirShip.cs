using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * Flying airships. Very similar to Maktakien boats (see Boat) but these do fly :P
 * @author DrHouse, DS
 */
public class AirShip : Vehicle
{
	public AirShip(CreatureTemplate template):base(template)
	{
		setInstanceType(InstanceType.AirShip);
		setAI(new AirShipAI(this));
	}
	
	public override bool isAirShip()
	{
		return true;
	}
	
	public virtual bool isOwner(Player player)
	{
		return false;
	}
	
	public virtual int getOwnerId()
	{
		return 0;
	}
	
	public virtual bool isCaptain(Player player)
	{
		return false;
	}
	
	public virtual int getCaptainId()
	{
		return 0;
	}
	
	public virtual int getHelmObjectId()
	{
		return 0;
	}
	
	public virtual int getHelmItemId()
	{
		return 0;
	}
	
	public virtual bool setCaptain(Player player)
	{
		return false;
	}
	
	public virtual int getFuel()
	{
		return 0;
	}
	
	public virtual void setFuel(int f)
	{
	}
	
	public virtual int getMaxFuel()
	{
		return 0;
	}
	
	public virtual void setMaxFuel(int mf)
	{
	}
	
	public override int getId()
	{
		return 0;
	}
	
	public override bool moveToNextRoutePoint()
	{
		bool result = base.moveToNextRoutePoint();
		if (result)
		{
			broadcastPacket(new ExMoveToLocationAirShipPacket(this));
		}
		return result;
	}
	
	public override bool addPassenger(Player player)
	{
		if (!base.addPassenger(player))
		{
			return false;
		}
		
		player.setVehicle(this);
		player.setInVehiclePosition(default);
		player.broadcastPacket(new ExGetOnAirShipPacket(player, this));
		player.setXYZ(getX(), getY(), getZ());
		player.revalidateZone(true);
		player.stopMove(null);
		return true;
	}
	
	public override void oustPlayer(Player player)
	{
		base.oustPlayer(player);
		LocationHeading loc = getOustLoc();
		if (player.isOnline())
		{
			player.broadcastPacket(new ExGetOffAirShipPacket(player, this, loc.Location));
			player.teleToLocation(loc);
		}
		else
		{
			player.setXYZInvisible(loc.Location);
		}
	}
	
	public override bool deleteMe()
	{
		if (!base.deleteMe())
		{
			return false;
		}
		
		AirShipManager.getInstance().removeAirShip(this);
		return true;
	}
	
	public override void stopMove(LocationHeading? loc)
	{
		base.stopMove(loc);
		
		broadcastPacket(new ExStopMoveAirShipPacket(this));
	}
	
	public override void updateAbnormalVisualEffects()
	{
		broadcastPacket(new ExAirShipInfoPacket(this));
	}
	
	public override void sendInfo(Player player)
	{
		if (isVisibleFor(player))
		{
			player.sendPacket(new ExAirShipInfoPacket(this));
		}
	}
}