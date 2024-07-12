using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartySmallWindowAllPacket: IOutgoingPacket
{
    private readonly Party _party;
    private readonly Player _exclude;

    public PartySmallWindowAllPacket(Player exclude, Party party)
    {
        _exclude = exclude;
        _party = party;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_SMALL_WINDOW_ALL);

        writer.WriteInt32(_party.getLeaderObjectId());
        writer.WriteByte((byte)_party.getDistributionType());
        writer.WriteByte((byte)(_party.getMemberCount() - 1));

        foreach (Player member in _party.getMembers())
        {
            if ((member != null) && (member != _exclude))
            {
                writer.WriteInt32(member.getObjectId());
                writer.WriteString(member.getName());
                writer.WriteInt32((int)member.getCurrentCp()); // c4
                writer.WriteInt32(member.getMaxCp()); // c4
                writer.WriteInt32((int)member.getCurrentHp());
                writer.WriteInt32(member.getMaxHp());
                writer.WriteInt32((int)member.getCurrentMp());
                writer.WriteInt32(member.getMaxMp());
                writer.WriteInt32(member.getVitalityPoints());
                writer.WriteByte((byte)member.getLevel());
                writer.WriteInt16((short)member.getClassId());
                writer.WriteByte(1); // Unk
                writer.WriteInt16((short)member.getRace());
                writer.WriteInt32(0); // 228

                Summon pet = member.getPet();
                writer.WriteInt32(member.getServitors().Count + (pet != null ? 1 : 0)); // Summon size, one only atm
                if (pet != null)
                {
                    writer.WriteInt32(pet.getObjectId());
                    writer.WriteInt32(pet.getId() + 1000000);
                    writer.WriteByte((byte)pet.getSummonType());
                    writer.WriteString(pet.getName());
                    writer.WriteInt32((int)pet.getCurrentHp());
                    writer.WriteInt32(pet.getMaxHp());
                    writer.WriteInt32((int)pet.getCurrentMp());
                    writer.WriteInt32(pet.getMaxMp());
                    writer.WriteByte((byte)pet.getLevel());
                }

                ICollection<Summon> servitors = member.getServitors().Values;
                foreach (Summon servitor in servitors)
                {
                    writer.WriteInt32(servitor.getObjectId());
                    writer.WriteInt32(servitor.getId() + 1000000);
                    writer.WriteByte((byte)servitor.getSummonType());
                    writer.WriteString(servitor.getName());
                    writer.WriteInt32((int)servitor.getCurrentHp());
                    writer.WriteInt32(servitor.getMaxHp());
                    writer.WriteInt32((int)servitor.getCurrentMp());
                    writer.WriteInt32(servitor.getMaxMp());
                    writer.WriteByte((byte)servitor.getLevel());
                }
            }
        }
    }
}