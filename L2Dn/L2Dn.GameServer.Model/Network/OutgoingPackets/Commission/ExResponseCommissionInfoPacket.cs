using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionInfoPacket: IOutgoingPacket
{
    public static readonly ExResponseCommissionInfoPacket EMPTY = default;
	
    private readonly bool _success;
    private readonly int _itemId;
    private readonly long _presetPricePerUnit;
    private readonly long _presetAmount;
    private readonly int _presetDurationType;
	
    public ExResponseCommissionInfoPacket(int itemId, long presetPricePerUnit, long presetAmount, int presetDurationType)
    {
        _success = true;
        _itemId = itemId;
        _presetPricePerUnit = presetPricePerUnit;
        _presetAmount = presetAmount;
        _presetDurationType = presetDurationType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_INFO);
        writer.WriteInt32(_success);
        writer.WriteInt32(_itemId);
        writer.WriteInt64(_presetPricePerUnit);
        writer.WriteInt64(_presetAmount);
        writer.WriteInt32(_success ? _presetDurationType : -1);
    }
}