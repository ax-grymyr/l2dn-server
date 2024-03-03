using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShortBuffStatusUpdatePacket: IOutgoingPacket
{
    public static readonly ShortBuffStatusUpdatePacket RESET_SHORT_BUFF = default;
	
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _skillSubLevel;
    private readonly int _duration;
	
    public ShortBuffStatusUpdatePacket(int skillId, int skillLevel, int skillSubLevel, int duration)
    {
        _skillId = skillId;
        _skillLevel = skillLevel;
        _skillSubLevel = skillSubLevel;
        _duration = duration;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHORT_BUFF_STATUS_UPDATE);

        writer.WriteInt32(_skillId);
        writer.WriteInt16((short)_skillLevel);
        writer.WriteInt16((short)_skillSubLevel);
        writer.WriteInt32(_duration);
    }
}