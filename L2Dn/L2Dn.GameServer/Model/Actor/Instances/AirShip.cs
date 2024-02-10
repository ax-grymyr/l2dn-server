using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;

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
	
	public bool isOwner(Player player)
	{
		return false;
	}
	
	public int getOwnerId()
	{
		return 0;
	}
	
	public bool isCaptain(Player player)
	{
		return false;
	}
	
	public int getCaptainId()
	{
		return 0;
	}
	
	public int getHelmObjectId()
	{
		return 0;
	}
	
	public int getHelmItemId()
	{
		return 0;
	}
	
	public bool setCaptain(Player player)
	{
		return false;
	}
	
	public int getFuel()
	{
		return 0;
	}
	
	public void setFuel(int f)
	{
	}
	
	public int getMaxFuel()
	{
		return 0;
	}
	
	public void setMaxFuel(int mf)
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
		player.setInVehiclePosition(new Location(0, 0, 0));
		player.broadcastPacket(new ExGetOnAirShip(player, this));
		player.setXYZ(getX(), getY(), getZ());
		player.revalidateZone(true);
		player.stopMove(null);
		return true;
	}
	
	public override void oustPlayer(Player player)
	{
		base.oustPlayer(player);
		Location loc = getOustLoc();
		if (player.isOnline())
		{
			player.broadcastPacket(new ExGetOffAirShip(player, this, loc.getX(), loc.getY(), loc.getZ()));
			player.teleToLocation(loc.getX(), loc.getY(), loc.getZ());
		}
		else
		{
			player.setXYZInvisible(loc.getX(), loc.getY(), loc.getZ());
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
	
	public override void stopMove(Location loc)
	{
		base.stopMove(loc);
		
		broadcastPacket(new ExStopMoveAirShipPacket(this));
	}
	
	public override void updateAbnormalVisualEffects()
	{
		broadcastPacket(new ExAirShipInfo(this));
	}
	
	public override void sendInfo(Player player)
	{
		if (isVisibleFor(player))
		{
			player.sendPacket(new ExAirShipInfo(this));
		}
	}
}