using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Disarm by inventory slot effect implementation. At end of effect, it re-equips that item.
 * @author Nik
 */
public class Disarmor: AbstractEffect
{
	private readonly Map<int, int> _unequippedItems; // PlayerObjId, ItemObjId
	private readonly long _slot;
	
	public Disarmor(StatSet @params)
	{
		_unequippedItems = new();
		
		String slot = @params.getString("slot", "chest");
		_slot = ItemData.SLOTS.getOrDefault(slot, (long) ItemTemplate.SLOT_NONE);
		if (_slot == ItemTemplate.SLOT_NONE)
		{
			LOGGER.Error("Unknown bodypart slot for effect: " + slot);
		}
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (_slot != ItemTemplate.SLOT_NONE) && effected.isPlayer();
	}
	
	public override void continuousInstant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		Player player = effected.getActingPlayer();
		List<Item> unequipped = player.getInventory().unEquipItemInBodySlotAndRecord(_slot);
		if (!unequipped.isEmpty())
		{
			InventoryUpdatePacket iu = new InventoryUpdatePacket(unequipped.Select(x => new ItemInfo(x, ItemChangeType.MODIFIED)).ToList());
			player.sendInventoryUpdate(iu);
			player.broadcastUserInfo();
			
			SystemMessagePacket sm;
			Item unequippedItem = unequipped.get(0);
			if (unequippedItem.getEnchantLevel() > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(unequippedItem.getEnchantLevel());
				sm.Params.addItemName(unequippedItem);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
				sm.Params.addItemName(unequippedItem);
			}
			
			player.sendPacket(sm);
			effected.getInventory().blockItemSlot(_slot);
			_unequippedItems.put(effected.getObjectId(), unequippedItem.getObjectId());
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		int disarmedObjId = _unequippedItems.remove(effected.getObjectId());
		if ((disarmedObjId != null) && (disarmedObjId > 0))
		{
			Player player = effected.getActingPlayer();
			player.getInventory().unblockItemSlot(_slot);
			
			Item item = player.getInventory().getItemByObjectId(disarmedObjId);
			if (item != null)
			{
				player.getInventory().equipItem(item);
				InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(item, ItemChangeType.MODIFIED));
				player.sendInventoryUpdate(iu);
				if (item.isEquipped())
				{
					SystemMessagePacket sm;
					if (item.getEnchantLevel() > 0)
					{
						sm = new SystemMessagePacket(SystemMessageId.S1_S2_EQUIPPED);
						sm.Params.addInt(item.getEnchantLevel());
						sm.Params.addItemName(item);
					}
					else
					{
						sm = new SystemMessagePacket(SystemMessageId.S1_EQUIPPED);
						sm.Params.addItemName(item);
					}
					
					player.sendPacket(sm);
				}
			}
		}
	}
}