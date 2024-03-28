using System.Globalization;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSetCastleSiegeTimePacket: IIncomingPacket<GameSession>
{
	private int _castleId;
	private DateTime _time;

	public void ReadContent(PacketBitReader reader)
	{
		_castleId = reader.ReadInt32();
		_time = DateTime.UnixEpoch.AddSeconds(reader.ReadInt32());
	}

	public ValueTask ProcessAsync(Connection connection, GameSession session)
	{
		Player? player = session.Player;
		if (player is null)
			return ValueTask.CompletedTask;

		Castle castle = CastleManager.getInstance().getCastleById(_castleId);
		if (castle == null)
		{
			PacketLogger.Instance.Warn(GetType().Name + ": activeChar: " + player + " castle: " + castle +
			                           " castleId: " + _castleId);

			return ValueTask.CompletedTask;
		}

		if (castle.getOwnerId() > 0 && castle.getOwnerId() != player.getClanId())
		{
			PacketLogger.Instance.Warn(GetType().Name + ": activeChar: " + player + " castle: " + castle +
			                           " castleId: " + _castleId +
			                           " is trying to change siege date of not his own castle!");

			return ValueTask.CompletedTask;
		}

		if (!player.isClanLeader())
		{
			PacketLogger.Instance.Warn(GetType().Name + ": activeChar: " + player + " castle: " + castle +
			                           " castleId: " + _castleId +
			                           " is trying to change siege date but is not clan leader!");

			return ValueTask.CompletedTask;
		}

		if (castle.isTimeRegistrationOver())
		{
			PacketLogger.Instance.Warn(GetType().Name + ": activeChar: " + player + " castle: " + castle +
			                           " castleId: " + _castleId +
			                           " is trying to change siege date but currently not possible!");

			return ValueTask.CompletedTask;
		}

		if (!isSiegeTimeValid(castle.getSiegeDate(), _time))
		{
			PacketLogger.Instance.Warn(GetType().Name + ": activeChar: " + player + " castle: " + castle +
			                           " castleId: " + _castleId + " is trying to an invalid time (" +
			                           _time + " !");

			return ValueTask.CompletedTask;
		}

		castle.setSiegeDate(_time);
		castle.setTimeRegistrationOver(true);
		castle.getSiege().saveSiegeDate();
		SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.S1_HAS_ANNOUNCED_THE_NEXT_CASTLE_SIEGE_TIME);
		msg.Params.addCastleId(_castleId);
		Broadcast.toAllOnlinePlayers(msg);
		player.sendPacket(new SiegeInfoPacket(castle, player));

		return ValueTask.CompletedTask;
	}

	private static bool isSiegeTimeValid(DateTime siegeDate, DateTime chosenDate)
	{
		foreach (int hour in Config.SIEGE_HOUR_LIST)
		{
			DateTime cal1 = new DateTime(siegeDate.Year, siegeDate.Month, siegeDate.Day, hour, 0, 0);
			if (cal1 == chosenDate)
				return true;
		}

		return false;
	}
}