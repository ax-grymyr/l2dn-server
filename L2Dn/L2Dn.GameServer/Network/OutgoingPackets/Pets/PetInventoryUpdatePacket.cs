using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * @author Yme, Advi, UnAfraid
 */
public readonly struct PetInventoryUpdatePacket: IOutgoingPacket
{
	private readonly InventoryPacketHelper _helper;
	
	public PetInventoryUpdatePacket(Item item)
	{
		_helper = new InventoryPacketHelper();
		_helper.Items.Add(new ItemInfo(item));
	}
	
	public PetInventoryUpdatePacket(ItemInfo item)
	{
		_helper = new InventoryPacketHelper();
		_helper.Items.Add(item);
	}
	
	public PetInventoryUpdatePacket(List<ItemInfo> items)
	{
		_helper = new InventoryPacketHelper();
		_helper.Items.AddRange(items);
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.PET_INVENTORY_UPDATE);
		if (_helper is null)
			writer.WriteInt32(0); // count
		else
			_helper.WriteItems(writer);
	}
}