using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Appearance;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author UnAfraid
 */
public class Appearance: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		if (player.hasRequest<ShapeShiftingItemRequest>())
		{
			player.sendPacket(SystemMessageId.APPEARANCE_MODIFICATION_OR_RESTORATION_IN_PROGRESS_PLEASE_TRY_AGAIN_AFTER_COMPLETING_THIS_TASK);
			return false;
		}

		AppearanceStone? stone = AppearanceItemData.getInstance().getStone(item.Id);
		if (stone == null)
		{
			player.sendMessage("This item is either not an appearance stone or is currently not handled!");
			return false;
		}

		player.addRequest(new ShapeShiftingItemRequest(player, item));
		player.sendPacket(new ExChooseShapeShiftingItemPacket(stone));
		return true;
	}
}