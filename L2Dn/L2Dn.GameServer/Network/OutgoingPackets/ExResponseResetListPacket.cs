using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExResponseResetListPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExResponseResetListPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_RESET_LIST);
        
        writer.WriteInt64(_player.getAdena());
        writer.WriteInt64(_player.getBeautyTickets());
        writer.WriteInt32(_player.getAppearance().getHairStyle());
        writer.WriteInt32(_player.getAppearance().getHairColor());
        writer.WriteInt32(_player.getAppearance().getFace());
    }
}