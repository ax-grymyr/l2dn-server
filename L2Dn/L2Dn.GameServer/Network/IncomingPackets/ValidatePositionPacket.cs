using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ValidatePositionPacket: IIncomingPacket<GameSession>
{
    private int _x;
    private int _y;
    private int _z;
    private int _heading;
    private int _vehicleId;

    public void ReadContent(PacketBitReader reader)
    {
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();
        _z = reader.ReadInt32();
        _heading = reader.ReadInt32();
        _vehicleId = reader.ReadInt32(); // vehicle id
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null || player.isTeleporting() || player.inObserverMode() || player.isCastingNow())
			return ValueTask.CompletedTask;
		
		int realX = player.getX();
		int realY = player.getY();
		int realZ = player.getZ();
		if (_x == 0 && _y == 0 && realX != 0)
			return ValueTask.CompletedTask;
		
		if (player.isInVehicle())
			return ValueTask.CompletedTask;
		
		if (player.isFalling(_z))
			return ValueTask.CompletedTask; // Disable validations during fall to avoid "jumping".
		
		// Don't allow flying transformations outside gracia area!
		if (player.isFlyingMounted() && _x > World.GRACIA_MAX_X)
		{
			player.untransform();
		}
		
		int dx = _x - realX;
		int dy = _y - realY;
		int dz = _z - realZ;
		double diffSq = ((dx * dx) + (dy * dy));
		if (player.isFlying() || player.isInsideZone(ZoneId.WATER))
		{
			player.setXYZ(realX, realY, _z);
			if (diffSq > 90000)
			{
				connection.Send(new ValidateLocationPacket(player));
			}
		}
		else if (diffSq < 360000) // If too large, messes observation.
		{
			if (diffSq > 250000 || Math.Abs(dz) > 200)
			{
				if ((Math.Abs(dz) > 200) && (Math.Abs(dz) < 1500) && (Math.Abs(_z - player.getClientZ()) < 800))
				{
					player.setXYZ(realX, realY, _z);
					realZ = _z;
				}
				else
				{
					connection.Send(new ValidateLocationPacket(player));
				}
			}
		}
		
		// Check out of sync.
		if (player.calculateDistance3D(_x, _y, _z) > player.getStat().getMoveSpeed())
		{
			if (player.isBlinkActive())
			{
				player.setBlinkActive(false);
			}
			else
			{
				player.setXYZ(_x, _y, player.getZ() > _z ? GeoEngine.getInstance().getHeight(_x, _y, player.getZ()) : _z);
			}
		}
		
		player.setClientX(_x);
		player.setClientY(_y);
		player.setClientZ(_z);
		player.setClientHeading(_heading); // No real need to validate heading.
		
		// Mobius: Check for possible door logout and move over exploit. Also checked at MoveBackwardToLocation.
		if (!DoorData.getInstance().checkIfDoorsBetween(realX, realY, realZ, _x, _y, _z, player.getInstanceWorld(), false))
		{
			player.setLastServerPosition(realX, realY, realZ);
		}

        return ValueTask.CompletedTask;
    }
}