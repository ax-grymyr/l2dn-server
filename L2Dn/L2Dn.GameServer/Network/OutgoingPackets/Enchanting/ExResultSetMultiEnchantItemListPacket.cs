using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExResultSetMultiEnchantItemListPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _resultType;
	
    public ExResultSetMultiEnchantItemListPacket(Player player, int resultType)
    {
        _player = player;
        _resultType = resultType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        if (_player.getRequest<EnchantItemRequest>() == null)
        {
            return;
        }
		
        writer.WritePacketCode(OutgoingPacketCodes.EX_RES_SET_MULTI_ENCHANT_ITEM_LIST);
        
        writer.WriteInt32(_resultType);
    }
}