using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

public class RollingDice: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		int itemId = item.Id;
		if (player.isInOlympiadMode())
		{
			player.sendPacket(SystemMessageId.THE_ITEM_CANNOT_BE_USED_IN_THE_OLYMPIAD);
			return false;
		}

		int number = rollDice(player);
		if (number == 0)
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_THROW_THE_DICE_AT_THIS_TIME_TRY_AGAIN_LATER);
			return false;
		}

		// Mobius: Retail dice position land calculation.
		double angle = HeadingUtil.ConvertHeadingToDegrees(player.getHeading());
		double radian = double.DegreesToRadians(angle);
		double course = double.DegreesToRadians(180);
		int x1 = (int) (Math.Cos(Math.PI + radian + course) * 40);
		int y1 = (int) (Math.Sin(Math.PI + radian + course) * 40);
		int x = player.getX() + x1;
		int y = player.getY() + y1;
		int z = player.getZ();
		Location3D destination = GeoEngine.getInstance().getValidLocation(player.Location.Location3D, new Location3D(x, y, z), player.getInstanceWorld());
		Broadcast.toSelfAndKnownPlayers(player, new DicePacket(player.ObjectId, itemId, number, destination));

		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_ROLLED_A_S2);
		sm.Params.addString(player.getName());
		sm.Params.addInt(number);

		player.sendPacket(sm);
		if (player.isInsideZone(ZoneId.PEACE))
		{
			Broadcast.toKnownPlayers(player, sm);
		}
		else if (player.isInParty()) // TODO: Verify this!
		{
			player.getParty()?.broadcastToPartyMembers(player, sm);
		}
		return true;
	}

	private static int rollDice(Player player)
	{
		// TODO: flood protection
		// Check if the dice is ready
		// if (!player.getClient().getFloodProtectors().canRollDice())
		// {
		// 	return 0;
		// }

		return Rnd.get(1, 6); // TODO: upper bound is not inclusive
	}
}