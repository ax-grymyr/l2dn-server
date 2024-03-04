using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SellListProcurePacket: IOutgoingPacket
{
    private readonly long _money;
    private readonly Map<Item, long> _sellList;
	
    public SellListProcurePacket(Player player, int castleId)
    {
        _money = player.getAdena();
        _sellList = new Map<Item, long>();
        foreach (CropProcure c in CastleManorManager.getInstance().getCropProcure(castleId, false))
        {
            Item item = player.getInventory().getItemByItemId(c.getId());
            if ((item != null) && (c.getAmount() > 0))
            {
                _sellList.put(item, c.getAmount());
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SELL_LIST_PROCURE);
        
        writer.WriteInt64(_money); // money
        writer.WriteInt32(0); // lease ?
        writer.WriteInt16((short)_sellList.size()); // list size
        foreach (var entry in _sellList)
        {
            Item item = entry.Key;
            writer.WriteInt16((short)item.getTemplate().getType1());
            writer.WriteInt32(item.getObjectId());
            writer.WriteInt32(item.getDisplayId());
            writer.WriteInt64(entry.Value); // count
            writer.WriteInt16((short)item.getTemplate().getType2());
            writer.WriteInt16(0); // unknown
            writer.WriteInt64(0); // price, you should not get any adena for crops, only raw materials
        }
    }
}