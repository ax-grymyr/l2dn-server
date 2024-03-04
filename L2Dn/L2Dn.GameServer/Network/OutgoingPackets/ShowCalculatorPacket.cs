using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShowCalculatorPacket: IOutgoingPacket
{
    private readonly int _calculatorId;
	
    public ShowCalculatorPacket(int calculatorId)
    {
        _calculatorId = calculatorId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOW_CALC);

        writer.WriteInt32(_calculatorId);
    }
}