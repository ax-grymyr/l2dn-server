using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRequestChangeNicknameColorPacket: IOutgoingPacket
{
    private readonly int _itemId;
	
    public ExRequestChangeNicknameColorPacket(int itemId)
    {
        _itemId = itemId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_NICKNAME_NCOLOR);
        
        writer.WriteInt32(_itemId);
    }
}