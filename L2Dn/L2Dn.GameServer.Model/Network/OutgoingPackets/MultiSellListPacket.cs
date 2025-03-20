using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct MultiSellListPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly PreparedMultisellListHolder _list;
    private readonly int _index;
    private readonly int _type;

    public MultiSellListPacket(Player player, PreparedMultisellListHolder list, int index, int type)
    {
        _player = player;
        _list = list;
        _index = index;
        _type = type;
    }

    public void WriteContent(PacketBitWriter writer)
    {
	    int size = _list.getEntries().Length - _index;
	    bool finished = true;
	    if (size > MultisellData.PAGE_SIZE)
	    {
		    finished = false;
		    size = MultisellData.PAGE_SIZE;
	    }

	    writer.WritePacketCode(OutgoingPacketCodes.MULTI_SELL_LIST);
		writer.WriteByte(0); // Helios
		writer.WriteInt32(_list.Id); // list id
		writer.WriteByte((byte)_type); // 196?
		writer.WriteInt32(1 + _index / MultisellData.PAGE_SIZE); // page started from 1
		writer.WriteInt32(finished); // finished
		writer.WriteInt32(MultisellData.PAGE_SIZE); // size of pages

		writer.WriteInt32(size); // list length
		writer.WriteByte(0); // Grand Crusade
		writer.WriteByte(_list.isChanceMultisell()); // new multisell window
		writer.WriteInt32(32); // Helios - Always 32

		for (int index = _index, end = _index + size; index < end; index++)
		{
			ItemInfo? itemEnchantment = _list.getItemEnchantment(index);
			MultisellEntryHolder entry = _list.getEntries()[index];
			if (itemEnchantment == null && _list.isMaintainEnchantment())
			{
				foreach (ItemChanceHolder holder in entry.getIngredients())
				{
					Item? item = _player.getInventory().getItemByItemId(holder.Id);
					if (item != null && item.isEquipable())
					{
						itemEnchantment = new ItemInfo(item);
						break;
					}
				}
			}
			writer.WriteInt32(index); // Entry ID. Start from 1.
			writer.WriteByte(entry.isStackable());
			// Those values will be passed down to MultiSellChoose packet.
			writer.WriteInt16((short)(itemEnchantment?.getEnchantLevel() ?? 0)); // enchant level
			writer.WriteItemAugment(itemEnchantment);
			writer.WriteItemElemental(itemEnchantment);
			writer.WriteItemEnsoulOptions(itemEnchantment);
			writer.WriteByte(0); // 286
			writer.WriteInt16((short)entry.getProducts().Count);
			writer.WriteInt16((short)entry.getIngredients().Count);
			foreach (ItemChanceHolder product in entry.getProducts())
			{
				ItemTemplate? template = ItemData.getInstance().getTemplate(product.Id);
				ItemInfo? displayItemEnchantment =
					_list.isMaintainEnchantment() && itemEnchantment != null && template != null &&
					template.GetType() == itemEnchantment.getItem().GetType()
						? itemEnchantment
						: null;
				if (template != null)
				{
					writer.WriteInt32(template.getDisplayId());
					writer.WriteInt64(template.getBodyPart());
					writer.WriteInt16((short)template.getType2());
				}
				else
				{
					writer.WriteInt32(product.Id);
					writer.WriteInt64(0);
					writer.WriteInt16(-1);
				}

				writer.WriteInt64(_list.getProductCount(product));
				writer.WriteInt16((short)(product.getEnchantmentLevel() > 0
					? product.getEnchantmentLevel()
					: displayItemEnchantment?.getEnchantLevel() ?? 0)); // enchant level

				writer.WriteInt32((int)(product.getChance() * 1000000)); // chance
				writer.WriteItemAugment(displayItemEnchantment);
				writer.WriteItemElemental(displayItemEnchantment);
				writer.WriteItemEnsoulOptions(displayItemEnchantment);
				writer.WriteByte(0); // 286
			}

			foreach (ItemChanceHolder ingredient in entry.getIngredients())
			{
				ItemTemplate? template = ItemData.getInstance().getTemplate(ingredient.Id);
				ItemInfo? displayItemEnchantment = itemEnchantment != null && template != null &&
				                                  template.GetType() == itemEnchantment.GetType()
					? itemEnchantment
					: null;

				if (template != null)
				{
					writer.WriteInt32(template.getDisplayId());
					writer.WriteInt16((short)template.getType2());
				}
				else
				{
					writer.WriteInt32(ingredient.Id);
					writer.WriteInt16(-1);
				}

				writer.WriteInt64(_list.getIngredientCount(ingredient));
				writer.WriteInt16((short)(ingredient.getEnchantmentLevel() > 0 ? ingredient.getEnchantmentLevel() :
					displayItemEnchantment?.getEnchantLevel() ?? 0)); // enchant level

				writer.WriteItemAugment(displayItemEnchantment);
				writer.WriteItemElemental(displayItemEnchantment);
				writer.WriteItemEnsoulOptions(displayItemEnchantment);
				writer.WriteByte(0); // 286
			}
		}
    }
}