using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExItemAnnouncePacket: IOutgoingPacket
{
    public const int ENCHANT = 0;
    public const int RANDOM_CRAFT = 2;
    public const int SPECIAL_CREATION = 3;
    public const int COMPOUND = 8;
    public const int UPGRADE = 10;

    private readonly Item _item;
    private readonly int _type;
    private readonly string _announceName;

    public ExItemAnnouncePacket(Player player, Item item, int type)
    {
        _item = item;
        _type = type;
        if (player.getClientSettings().isAnnounceEnabled())
            _announceName = player.getName();
        else if ("ru".equals(player.getLang()))
            _announceName = "Некто";
        else
            _announceName = "Someone";
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ITEM_ANNOUNCE);

        // _type
        // 0 - enchant
        // 1 - item get from container
        // 2 - item get from random creation
        // 3 - item get from special creation
        // 4 - item get from private workshop
        // 5 - item get from secret shop
        // 6 - item get from limited craft
        // 7 - fire and item get from container
        // 8 - item get from compound
        // 9 - item get from craft system but fancy
        // 10 - item get from upgrade
        // 11 and others - null item name by item_id and icon from chest.
        writer.WriteByte((byte)_type); // announce type
        writer.WriteSizedString(_announceName); // name of player
        writer.WriteInt32(_item.Id); // item id
        writer.WriteByte((byte)_item.getEnchantLevel()); // enchant level
        writer.WriteInt32(0); // chest item id
    }
}