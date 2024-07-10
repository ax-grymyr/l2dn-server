using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.AttributeChange;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author Mobius
 */
public class ChangeAttributeCrystal: IItemHandler
{
	private static readonly Map<int, ItemGrade> ITEM_GRADES = new();
	
	static ChangeAttributeCrystal()
	{
		ITEM_GRADES.put(33502, ItemGrade.S);
		ITEM_GRADES.put(35749, ItemGrade.R);
		ITEM_GRADES.put(45817, ItemGrade.R);
	}
	
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}
		
		Player player = playable.getActingPlayer();
		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CHANGE_AN_ATTRIBUTE_WHILE_USING_A_PRIVATE_STORE_OR_WORKSHOP);
			return false;
		}
		
		if (ITEM_GRADES.get(item.getId()) == null)
		{
			player.sendPacket(SystemMessageId.CHANGING_ATTRIBUTES_HAS_BEEN_FAILED);
			return false;
		}
		
		List<ItemInfo> itemList = new();
		foreach (Item i in player.getInventory().getItems())
		{
			if (i.isWeapon() && i.hasAttributes() && (i.getTemplate().getItemGrade() == ITEM_GRADES.get(item.getId())))
			{
				itemList.add(new ItemInfo(i));
			}
		}
		
		if (itemList.Count == 0)
		{
			player.sendPacket(SystemMessageId.THE_ITEM_FOR_CHANGING_AN_ATTRIBUTE_DOES_NOT_EXIST);
			return false;
		}
		
		player.sendPacket(new ExChangeAttributeItemListPacket(item.getId(), itemList));
		return true;
	}
}