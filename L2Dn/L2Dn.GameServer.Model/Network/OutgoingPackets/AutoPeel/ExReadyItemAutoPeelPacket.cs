using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;

public readonly struct ExReadyItemAutoPeelPacket: IOutgoingPacket
{
    private readonly int _itemObjectId;
    private readonly bool _result;
	
    public ExReadyItemAutoPeelPacket(bool result, int itemObjectId)
    {
        _result = result;
        _itemObjectId = itemObjectId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_READY_ITEM_AUTO_PEEL);
        
        writer.WriteByte(_result);
        writer.WriteInt32(_itemObjectId);
    }
}