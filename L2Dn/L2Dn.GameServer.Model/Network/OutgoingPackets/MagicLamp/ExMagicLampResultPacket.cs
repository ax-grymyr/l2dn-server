using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.MagicLamp;

public readonly struct ExMagicLampResultPacket: IOutgoingPacket
{
    private readonly long _exp;
    private readonly LampType _grade;
	
    public ExMagicLampResultPacket(long exp, LampType grade)
    {
        _exp = exp;
        _grade = grade;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MAGICLAMP_RESULT);
        writer.WriteInt64(_exp);
        writer.WriteInt32((int)_grade);
    }
}