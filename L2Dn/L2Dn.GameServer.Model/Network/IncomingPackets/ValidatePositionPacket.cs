using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ValidatePositionPacket: IIncomingPacket<GameSession>
{
    private Location _location;
    private int _vehicleId;

    public void ReadContent(PacketBitReader reader)
    {
        _location = reader.ReadLocation();
        _vehicleId = reader.ReadInt32(); // vehicle id
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null || player.isTeleporting() || player.inObserverMode() || player.isCastingNow())
			return ValueTask.CompletedTask;
		
		Location3D realLocation = player.Location.Location3D;
		if (_location.X == 0 && _location.Y == 0 && realLocation.X != 0)
			return ValueTask.CompletedTask;
		
		if (player.isInVehicle())
			return ValueTask.CompletedTask;
		
		if (player.isFalling(_location.Z))
			return ValueTask.CompletedTask; // Disable validations during fall to avoid "jumping".
		
		// Don't allow flying transformations outside gracia area!
		if (player.isFlyingMounted() && _location.X > World.GRACIA_MAX_X)
		{
			player.untransform();
		}
		
		Location3D dLoc = _location.Location3D - realLocation;
		double diffSq = dLoc.SquaredLength2D;
		if (player.isFlying() || player.isInsideZone(ZoneId.WATER))
		{
			player.setXYZ(realLocation.X, realLocation.Y, _location.Z);
			if (diffSq > 90000)
			{
				connection.Send(new ValidateLocationPacket(player));
			}
		}
		else if (diffSq < 360000) // If too large, messes observation.
		{
			if (diffSq > 250000 || Math.Abs(dLoc.Z) > 200)
			{
				if (Math.Abs(dLoc.Z) > 200 && Math.Abs(dLoc.Z) < 1500 && Math.Abs(_location.Z - player.getClientZ()) < 800)
				{
					player.setXYZ(realLocation.X, realLocation.Y, _location.Z);
					realLocation = realLocation with { Z = _location.Z };
				}
				else
				{
					connection.Send(new ValidateLocationPacket(player));
				}
			}
		}
		
		// Check out of sync.
		if (player.Distance3D(_location) > player.getStat().getMoveSpeed())
		{
			if (player.isBlinkActive())
			{
				player.setBlinkActive(false);
			}
			else
			{
				player.setXYZ(_location.X, _location.Y,
					player.getZ() > _location.Z
						? GeoEngine.getInstance().getHeight(_location.Location3D with { Z = player.getZ() })
						: _location.Z);
			}
		}
		
		player.setClientX(_location.X);
		player.setClientY(_location.Y);
		player.setClientZ(_location.Z);
		player.setClientHeading(_location.Heading); // No real need to validate heading.
		
		// Mobius: Check for possible door logout and move over exploit. Also checked at MoveBackwardToLocation.
		if (!DoorData.getInstance().checkIfDoorsBetween(realLocation, _location.Location3D,
			    player.getInstanceWorld(), false))
		{
			player.setLastServerPosition(realLocation);
		}

		return ValueTask.CompletedTask;
    }
}