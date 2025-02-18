using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CastleWar;

public readonly struct MercenaryCastleWarCastleSiegeDefenderListPacket: IOutgoingPacket
{
	private readonly int _castleId;

	public MercenaryCastleWarCastleSiegeDefenderListPacket(int castleId)
	{
		_castleId = castleId;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_MERCENARY_CASTLEWAR_CASTLE_SIEGE_DEFENDER_LIST);

		writer.WriteInt32(_castleId);
		writer.WriteInt32(0);
		writer.WriteInt32(1);
		writer.WriteInt32(0);

		Castle? castle = CastleManager.getInstance().getCastleById(_castleId);
		if (castle == null)
		{
			writer.WriteInt32(0);
			writer.WriteInt32(0);
		}
		else
		{
			int size = castle.getSiege().getDefenderWaitingClans().Count + castle.getSiege().getDefenderClans().Count + (castle.getOwner() != null ? 1 : 0);
			writer.WriteInt32(size);
			writer.WriteInt32(size);

			// Owners.
			Clan owner = castle.getOwner();
			if (owner != null)
			{
				writer.WriteInt32(owner.getId());
				writer.WriteString(owner.getName());
				writer.WriteString(owner.getLeaderName());
				writer.WriteInt32(owner.getAllyCrestId() ?? 0);
				writer.WriteInt32(0); // time (seconds)
				writer.WriteInt32((int)SiegeClanType.OWNER);

				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286

				writer.WriteInt32(owner.getAllyId() ?? 0);
				writer.WriteString(owner.getAllyName() ?? string.Empty);
				writer.WriteString(""); // Ally Leader Name
				writer.WriteInt32(owner.getAllyCrestId() ?? 0);
			}

			// Defenders.
			foreach (SiegeClan clan in castle.getSiege().getDefenderClans())
			{
				Clan? defender = ClanTable.getInstance().getClan(clan.getClanId());
				if ((defender == null) || (defender == castle.getOwner()))
				{
					continue;
				}

				writer.WriteInt32(defender.getId());
				writer.WriteString(defender.getName());
				writer.WriteString(defender.getLeaderName());
				writer.WriteInt32(defender.getCrestId() ?? 0);
				writer.WriteInt32(0); // time (seconds)
				writer.WriteInt32((int)SiegeClanType.DEFENDER);

				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286

				writer.WriteInt32(defender.getAllyId() ?? 0);
				writer.WriteString(defender.getAllyName() ?? string.Empty);
				writer.WriteString(""); // AllyLeaderName
				writer.WriteInt32(defender.getAllyCrestId() ?? 0);
			}

			// Defenders waiting.
			foreach (SiegeClan clan in castle.getSiege().getDefenderWaitingClans())
			{
				Clan? defender = ClanTable.getInstance().getClan(clan.getClanId());
				if (defender == null)
				{
					continue;
				}

				writer.WriteInt32(defender.getId());
				writer.WriteString(defender.getName());
				writer.WriteString(defender.getLeaderName());
				writer.WriteInt32(defender.getCrestId() ?? 0);
				writer.WriteInt32(0); // time (seconds)
				writer.WriteInt32((int)SiegeClanType.DEFENDER_PENDING);

				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286
				writer.WriteInt32(0); // 286

				writer.WriteInt32(defender.getAllyId() ?? 0);
				writer.WriteString(defender.getAllyName() ?? string.Empty);
				writer.WriteString(""); // AllyLeaderName
				writer.WriteInt32(defender.getAllyCrestId() ?? 0);
			}
		}
	}
}