using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.AttributeChange;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author Mobius
 */
public class ChangeAttributeCrystal: IItemHandler
{
    private static readonly FrozenDictionary<int, ItemGrade> _itemGrades =
        new[] { (33502, ItemGrade.S), (35749, ItemGrade.R), (45817, ItemGrade.R) }.
            ToFrozenDictionary(t => t.Item1, t => t.Item2);

	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CHANGE_AN_ATTRIBUTE_WHILE_USING_A_PRIVATE_STORE_OR_WORKSHOP);
			return false;
		}

		if (!_itemGrades.ContainsKey(item.Id))
		{
			player.sendPacket(SystemMessageId.CHANGING_ATTRIBUTES_HAS_BEEN_FAILED);
			return false;
		}

		List<ItemInfo> itemList = new();
		foreach (Item i in player.getInventory().getItems())
		{
			if (i.isWeapon() && i.hasAttributes() &&
                _itemGrades.TryGetValue(item.Id, out ItemGrade grade) &&
                i.getTemplate().getItemGrade() == grade)
			{
				itemList.Add(new ItemInfo(i));
			}
		}

		if (itemList.Count == 0)
		{
			player.sendPacket(SystemMessageId.THE_ITEM_FOR_CHANGING_AN_ATTRIBUTE_DOES_NOT_EXIST);
			return false;
		}

		player.sendPacket(new ExChangeAttributeItemListPacket(item.Id, itemList));
		return true;
	}
}