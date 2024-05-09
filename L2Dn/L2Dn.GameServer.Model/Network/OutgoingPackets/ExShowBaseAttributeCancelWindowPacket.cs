using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowBaseAttributeCancelWindowPacket: IOutgoingPacket
{
    private readonly List<Item> _items;

    public ExShowBaseAttributeCancelWindowPacket(Player player)
    {
        _items = new List<Item>();
        foreach (Item item in player.getInventory().getItems())
        {
            if (item.hasAttributes())
            {
                _items.Add(item);
            }
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_BASE_ATTRIBUTE_CANCEL_WINDOW);

        writer.WriteInt32(_items.Count);
        foreach (Item item in _items)
        {
            writer.WriteInt32(item.getObjectId());
            writer.WriteInt64(getPrice(item));
        }
    }

    /**
     * TODO: Unhardcode! Update prices for Top/Mid/Low S80/S84
     * @param item
     * @return
     */
    private static long getPrice(Item item) =>
        item.getTemplate().getCrystalType() switch
        {
            CrystalType.S => item.isWeapon() ? 50000 : 40000,
            CrystalType.S80 => item.isWeapon() ? 100000 : 80000,
            CrystalType.S84 => item.isWeapon() ? 200000 : 160000,
            _ => 0
        };
}