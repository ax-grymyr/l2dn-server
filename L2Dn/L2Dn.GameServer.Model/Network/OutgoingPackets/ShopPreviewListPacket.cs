using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Items;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShopPreviewListPacket: IOutgoingPacket
{
    private readonly int _listId;
    private readonly ICollection<Product> _list;
    private readonly long _money;

    public ShopPreviewListPacket(ProductList list, long currentMoney)
    {
        _listId = list.getListId();
        _list = list.getProducts();
        _money = currentMoney;
    }

    public ShopPreviewListPacket(ICollection<Product> lst, int listId, long currentMoney)
    {
        _listId = listId;
        _list = lst;
        _money = currentMoney;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOP_PREVIEW_LIST);

        writer.WriteInt32(5056);
        writer.WriteInt64(_money); // current money
        writer.WriteInt32(_listId);
        int newlength = 0;
        foreach (Product product in _list)
        {
            if (product.getItem().isEquipable())
            {
                newlength++;
            }
        }

        writer.WriteInt16((short)newlength);
        foreach (Product product in _list)
        {
            if (product.getItem().isEquipable())
            {
                writer.WriteInt32(product.getItemId());
                writer.WriteInt16((short)product.getItem().getType2()); // item type2
                if (product.getItem().getType1() != ItemTemplate.TYPE1_ITEM_QUESTITEM_ADENA)
                {
                    writer.WriteInt64(product.getItem().getBodyPart()); // rev 415 slot 0006-lr.ear 0008-neck 0030-lr.finger 0040-head 0080-?? 0100-l.hand 0200-gloves 0400-chest 0800-pants 1000-feet 2000-?? 4000-r.hand 8000-r.hand
                }
                else
                {
                    writer.WriteInt64(0); // rev 415 slot 0006-lr.ear 0008-neck 0030-lr.finger 0040-head 0080-?? 0100-l.hand 0200-gloves 0400-chest 0800-pants 1000-feet 2000-?? 4000-r.hand 8000-r.hand
                }

                writer.WriteInt64(Config.WEAR_PRICE);
            }
        }
    }
}