using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAttributeEnchantResultPacket: IOutgoingPacket
{
    private readonly int _result;
    private readonly bool _isWeapon;
    private readonly AttributeType _type;
    private readonly int _before;
    private readonly int _after;
    private readonly int _successCount;
    private readonly int _failedCount;
	
    public ExAttributeEnchantResultPacket(int result, bool isWeapon, AttributeType type, int before, int after, int successCount, int failedCount)
    {
        _result = result;
        _isWeapon = isWeapon;
        _type = type;
        _before = before;
        _after = after;
        _successCount = successCount;
        _failedCount = failedCount;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ATTRIBUTE_ENCHANT_RESULT);
        
        writer.WriteInt32(_result);
        writer.WriteByte(_isWeapon);
        writer.WriteInt16((short)_type);
        writer.WriteInt16((short)_before);
        writer.WriteInt16((short)_after);
        writer.WriteInt16((short)_successCount);
        writer.WriteInt16((short)_failedCount);
    }
}