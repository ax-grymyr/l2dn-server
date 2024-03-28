using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ActionFailedPacket: IOutgoingPacket
{
    public static readonly ActionFailedPacket STATIC_PACKET = default;

    private readonly int _castingType;

    public ActionFailedPacket(int castingType)
    {
        _castingType = castingType;
    }

    public ActionFailedPacket(SkillCastingType castingType)
    {
        _castingType = (int)castingType;
    }
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ACTION_FAIL);

        writer.WriteInt32(_castingType); // MagicSkillUse castingType
    }
}