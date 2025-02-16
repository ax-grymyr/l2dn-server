using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartySmallWindowUpdatePacket: IOutgoingPacket
{
    private readonly Player _member;
    private readonly PartySmallWindowUpdateType _type;
	
    public PartySmallWindowUpdatePacket(Player member, PartySmallWindowUpdateType type)
    {
        _member = member;
        _type = type; 
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_SMALL_WINDOW_UPDATE);
        
        writer.WriteInt32(_member.ObjectId);
        writer.WriteInt16((short)_type);

        if (_type.HasFlag(PartySmallWindowUpdateType.CURRENT_CP))
            writer.WriteInt32((int) _member.getCurrentCp()); // c4
        
        if (_type.HasFlag(PartySmallWindowUpdateType.MAX_CP))
            writer.WriteInt32(_member.getMaxCp()); // c4

        if (_type.HasFlag(PartySmallWindowUpdateType.CURRENT_HP))
            writer.WriteInt32((int) _member.getCurrentHp());

        if (_type.HasFlag(PartySmallWindowUpdateType.MAX_HP))
            writer.WriteInt32(_member.getMaxHp());

        if (_type.HasFlag(PartySmallWindowUpdateType.CURRENT_MP))
            writer.WriteInt32((int) _member.getCurrentMp());

        if (_type.HasFlag(PartySmallWindowUpdateType.MAX_MP))
            writer.WriteInt32(_member.getMaxMp());

        if (_type.HasFlag(PartySmallWindowUpdateType.LEVEL))
            writer.WriteByte((byte)_member.getLevel());

        if (_type.HasFlag(PartySmallWindowUpdateType.CLASS_ID))
            writer.WriteInt16((short)_member.getClassId());

        if (_type.HasFlag(PartySmallWindowUpdateType.PARTY_SUBSTITUTE))
            writer.WriteByte(0);

        if (_type.HasFlag(PartySmallWindowUpdateType.VITALITY_POINTS))
            writer.WriteInt32(_member.getVitalityPoints());
    }
}