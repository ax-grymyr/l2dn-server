using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

public class EnchantAttribute: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}
		
		Player player = playable.getActingPlayer();
		if (player.isCastingNow())
		{
			return false;
		}
		
		if (player.hasItemRequest())
		{
			player.sendPacket(SystemMessageId.ANOTHER_ENCHANTMENT_IS_IN_PROGRESS_PLEASE_COMPLETE_THE_PREVIOUS_TASK_THEN_TRY_AGAIN);
			return false;
		}
		
		player.addRequest(new EnchantItemAttributeRequest(player, item.getObjectId()));
		player.sendPacket(new ExChooseInventoryAttributeItemPacket(player, item));
		return true;
	}
}