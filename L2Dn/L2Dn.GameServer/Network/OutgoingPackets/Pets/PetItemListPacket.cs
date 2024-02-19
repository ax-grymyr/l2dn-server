using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

public readonly struct PetItemListPacket: IOutgoingPacket
{
	private readonly ICollection<Item> _items;
	
	public PetItemListPacket(ICollection<Item> items)
	{
		_items = items;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.PET_ITEM_LIST);
		writer.WriteInt16((short)_items.Count);
		foreach (Item item in _items)
			InventoryPacketHelper.WriteItem(writer, item);
	}
}