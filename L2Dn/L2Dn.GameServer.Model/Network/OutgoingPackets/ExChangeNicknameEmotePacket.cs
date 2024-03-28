using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExChangeNicknameEmotePacket: IOutgoingPacket
{
    private readonly int _itemId;
	
    public ExChangeNicknameEmotePacket(int itemId)
    {
        _itemId = itemId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_NICKNAME_COLOR_ICON);
        
        writer.WriteInt32(_itemId);
    }
}