using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RecipeShopMsgPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public RecipeShopMsgPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RECIPE_SHOP_MSG);
        
        writer.WriteInt32(_player.ObjectId);
        writer.WriteString(_player.getStoreName());
    }
}