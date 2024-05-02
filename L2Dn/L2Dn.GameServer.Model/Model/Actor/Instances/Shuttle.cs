using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Shuttles;
using L2Dn.GameServer.Network.OutgoingPackets.Shuttle;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Shuttle: Vehicle
{
	private ShuttleDataHolder _shuttleData;

	public Shuttle(CreatureTemplate template): base(template)
	{
		setInstanceType(InstanceType.Shuttle);
		setAI(new ShuttleAI(this));
	}

	public List<ShuttleStop> getStops()
	{
		return _shuttleData.getStops();
	}

	public void closeDoor(int id)
	{
		foreach (ShuttleStop stop in _shuttleData.getStops())
		{
			if (stop.getId() == id)
			{
				stop.closeDoor();
				break;
			}
		}
	}

	public void openDoor(int id)
	{
		foreach (ShuttleStop stop in _shuttleData.getStops())
		{
			if (stop.getId() == id)
			{
				stop.openDoor();
				break;
			}
		}
	}

	public override int getId()
	{
		return _shuttleData.getId();
	}

	public override bool addPassenger(Player player)
	{
		if (!base.addPassenger(player))
		{
			return false;
		}

		player.setVehicle(this);
		player.setInVehiclePosition(default);
		player.broadcastPacket(new ExShuttleGetOnPacket(player, this));
		player.setXYZ(getX(), getY(), getZ());
		player.revalidateZone(true);
		return true;
	}

	public void removePassenger(Player player, int x, int y, int z)
	{
		oustPlayer(player);
		if (player.isOnline())
		{
			player.broadcastPacket(new ExShuttleGetOffPacket(player, this, x, y, z));
			player.setXYZ(x, y, z);
			player.revalidateZone(true);
		}
		else
		{
			player.setXYZInvisible(new Location3D(x, y, z));
		}
	}

	public override void oustPlayers()
	{
		// TODO: does same as base method
		List<Player> passengers = _passengers.ToList();
		foreach (Player player in passengers)
		{
			_passengers.remove(player);
			if (player != null)
			{
				oustPlayer(player);
			}
		}
	}

	public override void sendInfo(Player player)
	{
		player.sendPacket(new ExShuttleInfoPacket(this));
	}

	public void broadcastShuttleInfo()
	{
		broadcastPacket(new ExShuttleInfoPacket(this));
	}

	public void setData(ShuttleDataHolder data)
	{
		_shuttleData = data;
	}

	public ShuttleDataHolder getShuttleData()
	{
		return _shuttleData;
	}
}