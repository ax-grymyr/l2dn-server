using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TradeStartPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly Player _player;
    private readonly Player _partner;
    private readonly ICollection<Item> _itemList;
    private readonly int _mask;

    public TradeStartPacket(int sendType, Player player)
    {
        _sendType = sendType;
        _player = player;
        _partner = player.getActiveTradeList().getPartner();
        _itemList = _player.getInventory().getAvailableItems(true,
            (_player.canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && Config.GM_TRADE_RESTRICTED_ITEMS), false);

        if (_partner != null)
        {
            if (player.getFriendList().Contains(_partner.ObjectId))
            {
                _mask |= 0x01;
            }

            if ((player.getClanId() > 0) && (_partner.getClanId() == _partner.getClanId()))
            {
                _mask |= 0x02;
            }

            if ((MentorManager.getInstance().getMentee(player.ObjectId, _partner.ObjectId) != null) ||
                (MentorManager.getInstance().getMentee(_partner.ObjectId, player.ObjectId) != null))
            {
                _mask |= 0x04;
            }

            if ((player.getAllyId() > 0) && (player.getAllyId() == _partner.getAllyId()))
            {
                _mask |= 0x08;
            }

            // Does not shows level
            if (_partner.isGM())
            {
                _mask |= 0x10;
            }
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        if ((_player.getActiveTradeList() == null) || (_partner == null))
        {
            return;
        }
		
        writer.WritePacketCode(OutgoingPacketCodes.TRADE_START);
     
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_itemList.Count);
            writer.WriteInt32(_itemList.Count);
            foreach (Item item in _itemList)
            {
                InventoryPacketHelper.WriteItem(writer, item);
            }
        }
        else
        {
            writer.WriteInt32(_partner.ObjectId);
            writer.WriteByte((byte)_mask); // some kind of mask
            if ((_mask & 0x10) == 0)
            {
                writer.WriteByte((byte)_partner.getLevel());
            }
        }
    }
}