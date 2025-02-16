using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartySmallWindowAddPacket: IOutgoingPacket
{
    private readonly Player _member;
    private readonly Party _party;

    public PartySmallWindowAddPacket(Player member, Party party)
    {
        _member = member;
        _party = party;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_SMALL_WINDOW_ADD);
        
        writer.WriteInt32(_party.getLeaderObjectId()); // c3
        writer.WriteInt32((int)_party.getDistributionType()); // c3
        writer.WriteInt32(_member.ObjectId);
        writer.WriteString(_member.getName());
        writer.WriteInt32((int)_member.getCurrentCp()); // c4
        writer.WriteInt32(_member.getMaxCp()); // c4
        writer.WriteInt32((int)_member.getCurrentHp());
        writer.WriteInt32(_member.getMaxHp());
        writer.WriteInt32((int)_member.getCurrentMp());
        writer.WriteInt32(_member.getMaxMp());
        writer.WriteInt32(_member.getVitalityPoints());
        writer.WriteByte((byte)_member.getLevel());
        writer.WriteInt16((short)_member.getClassId());
        writer.WriteByte(0);
        writer.WriteInt16((short)_member.getRace());
        writer.WriteInt32(0); // 228
    }
}