using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;

public readonly struct ExStopItemAutoPeelPacket: IOutgoingPacket
{
    private readonly bool _result;

    public ExStopItemAutoPeelPacket(bool result)
    {
        _result = result;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STOP_ITEM_AUTO_PEEL);
        
        writer.WriteByte(_result);
    }
}