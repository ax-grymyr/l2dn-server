using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SiegeDefenderListPacket: IOutgoingPacket
{
	private readonly Castle _castle;
	
	public SiegeDefenderListPacket(Castle castle)
	{
		_castle = castle;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CASTLE_SIEGE_DEFENDER_LIST);
		
		writer.WriteInt32(_castle.getResidenceId());
		writer.WriteInt32(0); // Unknown.
		
		Clan owner = _castle.getOwner();
		writer.WriteInt32((owner != null) && _castle.isTimeRegistrationOver()); // Valid registration.
		writer.WriteInt32(0); // Unknown.
		
		// Add owners.
		List<Clan> defenders = new();
		if (owner != null)
		{
			defenders.Add(owner);
		}
		
		// List of confirmed defenders.
		foreach (SiegeClan siegeClan in _castle.getSiege().getDefenderClans())
		{
			Clan clan = ClanTable.getInstance().getClan(siegeClan.getClanId());
			if ((clan != null) && (clan != owner))
			{
				defenders.Add(clan);
			}
		}
		
		// List of not confirmed defenders.
		foreach (SiegeClan siegeClan in _castle.getSiege().getDefenderWaitingClans())
		{
			Clan clan = ClanTable.getInstance().getClan(siegeClan.getClanId());
			if (clan != null)
			{
				defenders.Add(clan);
			}
		}
		
		int size = defenders.Count;
		writer.WriteInt32(size);
		writer.WriteInt32(size);
		
		foreach (Clan clan in defenders)
		{
			writer.WriteInt32(clan.getId());
			writer.WriteString(clan.getName());
			writer.WriteString(clan.getLeaderName());
			writer.WriteInt32(clan.getCrestId() ?? 0);
			writer.WriteInt32(0); // Signed time in seconds.
			if (clan == owner)
			{
				writer.WriteInt32((int)SiegeClanType.OWNER + 1);
			}
			else if (_castle.getSiege().getDefenderClans().Any(defender => defender.getClanId() == clan.getId()))
			{
				writer.WriteInt32((int)SiegeClanType.DEFENDER + 1);
			}
			else
			{
				writer.WriteInt32((int)SiegeClanType.DEFENDER_PENDING + 1);
			}
			writer.WriteInt32(clan.getAllyId() ?? 0);
			if (clan.getAllyId() != 0)
			{
				AllianceInfo info = new AllianceInfo(clan.getAllyId());
				writer.WriteString(info.getName());
				writer.WriteString(info.getLeaderP()); // Ally leader name.
				writer.WriteInt32(clan.getAllyCrestId() ?? 0);
			}
			else
			{
				writer.WriteString("");
				writer.WriteString(""); // Ally leader name.
				writer.WriteInt32(0);
			}
		}
	}
}