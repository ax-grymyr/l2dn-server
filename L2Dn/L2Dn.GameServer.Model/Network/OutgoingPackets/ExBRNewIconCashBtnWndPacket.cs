using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBRNewIconCashBtnWndPacket: IOutgoingPacket
{
    private readonly short _active;
	
    public ExBRNewIconCashBtnWndPacket(short active)
    {
        _active = active;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_NEW_ICON_CASH_BTN_WND);
        
        writer.WriteInt16(_active);
    }
}