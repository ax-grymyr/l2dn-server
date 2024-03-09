using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

public class Book: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}
		
		Player player = (Player) playable;
		int itemId = item.getId();
		String filename = "data/html/help/" + itemId + ".htm";
		String content = HtmCache.getInstance().getHtm(player, filename);
		if (content == null)
		{
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, item.getId(), "<html><body>My Text is missing:<br>" + filename + "</body></html>");
			player.sendPacket(html);
		}
		else
		{
			NpcHtmlMessagePacket itemReply = new NpcHtmlMessagePacket(content);
			player.sendPacket(itemReply);
		}
		
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		return true;
	}
}