using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BeautyShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExResponseBeautyListPacket: IOutgoingPacket
{
    public const int SHOW_FACESHAPE = 1;
    public const int SHOW_HAIRSTYLE = 0;
	
    private readonly Player _player;
    private readonly int _type;
    private readonly Map<int, BeautyItem> _beautyItem;
	
    public ExResponseBeautyListPacket(Player player, int type)
    {
        _player = player;
        _type = type;
        if (type == SHOW_HAIRSTYLE)
        {
            _beautyItem = BeautyShopData.getInstance().getBeautyData(player.getRace(), player.getAppearance().getSex()).getHairList();
        }
        else
        {
            _beautyItem = BeautyShopData.getInstance().getBeautyData(player.getRace(), player.getAppearance().getSex()).getFaceList();
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_BEAUTY_LIST);
        
        writer.WriteInt64(_player.getAdena());
        writer.WriteInt64(_player.getBeautyTickets());
        writer.WriteInt32(_type);
        writer.WriteInt32(_beautyItem.Count);
        foreach (BeautyItem item in _beautyItem.Values)
        {
            writer.WriteInt32(item.getId());
            writer.WriteInt32(1); // Limit
        }
        
        writer.WriteInt32(0);
    }
}