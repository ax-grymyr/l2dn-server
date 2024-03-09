using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.ItemHandlers;

/**
 * @author Mode
 */
public class LimitedSayha: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}
		
		Player player = playable.getActingPlayer();
		long time = 0;
		switch (item.getId())
		{
			case 71899:
			{
				time = 86400000L * 30;
				break;
			}
			case 71900:
			{
				time = 86400000L * 1;
				break;
			}
			case 71901:
			{
				time = 86400000L * 7;
				break;
			}
			default:
			{
				time = 0;
				break;
			}
		}
		if ((time > 0) && player.setLimitedSayhaGraceEndTime(DateTime.UtcNow + TimeSpan.FromMilliseconds(time)))
		{
			player.destroyItem("LimitedSayha potion", item, 1, player, true);
		}
		else
		{
			player.sendMessage("Your Limited Sayha's Grace remaining time is greater than item's.");
			return false;
		}
		return true;
	}
}