using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.SteadyBoxes;

public readonly struct ExSteadyBoxRewardPacket: IOutgoingPacket
{
    private readonly int _slotId;
    private readonly int _itemId;
    private readonly long _itemCount;
	
    public ExSteadyBoxRewardPacket(int slotId, int itemId, long itemCount)
    {
        _slotId = slotId;
        _itemId = itemId;
        _itemCount = itemCount;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STEADY_BOX_REWARD);

        writer.WriteInt32(_slotId);
        writer.WriteInt32(_itemId);
        writer.WriteInt64(_itemCount);
        writer.WriteInt32(0);
    }
}