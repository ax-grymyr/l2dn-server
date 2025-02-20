using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct HennaEquipListPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly List<Henna> _hennaEquipList;
	
    public HennaEquipListPacket(Player player)
    {
        _player = player;
        _hennaEquipList = HennaData.getInstance().getHennaList(player);
    }
	
    public HennaEquipListPacket(Player player, List<Henna> list)
    {
        _player = player;
        _hennaEquipList = list;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.HENNA_EQUIP_LIST);
        
        writer.WriteInt64(_player.getAdena()); // activeChar current amount of Adena
        writer.WriteInt32(3); // available equip slot
        writer.WriteInt32(_hennaEquipList.Count);
        foreach (Henna henna in _hennaEquipList)
        {
            // Player must have at least one dye in inventory
            // to be able to see the Henna that can be applied with it.
            if (_player.getInventory().getItemByItemId(henna.getDyeItemId()) != null)
            {
                writer.WriteInt32(henna.getDyeId()); // dye Id
                writer.WriteInt32(henna.getDyeItemId()); // item Id of the dye
                writer.WriteInt64(henna.getWearCount()); // amount of dyes required
                writer.WriteInt64(henna.getWearFee()); // amount of Adena required
                writer.WriteInt32(henna.isAllowedClass(_player)); // meet the requirement or not
                // writer.WriteInt32(0); // Does not exist in Classic. // TODO: Classic!!!
            }
        }
    }
}