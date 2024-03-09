using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.ItemHandlers;

/**
 * Item skills not allowed on Olympiad.
 */
public class ItemSkills: ItemSkillsTemplate
{
	public override bool useItem(Playable playable, Item item, bool forceUse)
	{
		Player player = playable.getActingPlayer();
		if ((player != null) && player.isInOlympiadMode())
		{
			player.sendPacket(SystemMessageId.THE_ITEM_CANNOT_BE_USED_IN_THE_OLYMPIAD);
			return false;
		}

		return base.useItem(playable, item, forceUse);
	}
}