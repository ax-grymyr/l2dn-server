using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSaveInventoryOrderPacket: IIncomingPacket<GameSession>
{
    /** client limit */
    private const int LIMIT = 125;

    private List<InventoryOrder> _order;

    public void ReadContent(PacketBitReader reader)
    {
        int count = reader.ReadInt32();
        count = Math.Min(count, LIMIT);
        
        _order = new List<InventoryOrder>();
        for (int i = 0; i < count; i++)
        {
            int objectId = reader.ReadInt32();
            int order = reader.ReadInt32();
            _order.Add(new InventoryOrder(objectId, order));
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player != null && _order != null)
        {
            Inventory inventory = player.getInventory();
            foreach (InventoryOrder order in _order)
            {
                Item item = inventory.getItemByObjectId(order.ObjectId);
                if ((item != null) && (item.getItemLocation() == ItemLocation.INVENTORY))
                {
                    item.setItemLocation(ItemLocation.INVENTORY, order.Order);
                }
            }
        }
        
        return ValueTask.CompletedTask;
    }

    private readonly record struct InventoryOrder(int ObjectId, int Order);
}