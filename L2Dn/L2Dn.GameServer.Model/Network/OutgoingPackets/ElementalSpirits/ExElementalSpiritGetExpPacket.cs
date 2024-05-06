using L2Dn.GameServer.Enums;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ExElementalSpiritGetExpPacket: IOutgoingPacket
{
    private readonly long _experience;
    private readonly ElementalType _type;
	
    public ExElementalSpiritGetExpPacket(ElementalType type, long experience)
    {
        _type = type;
        _experience = experience;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_GET_EXP);
        
        writer.WriteByte((byte)_type);
        writer.WriteInt64(_experience);
    }
}