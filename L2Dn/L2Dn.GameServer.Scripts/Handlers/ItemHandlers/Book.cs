using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
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

		int itemId = item.Id;
		string filename = "html/help/" + itemId + ".htm";
		HtmlContent htmlContent = HtmlContent.LoadFromFile(filename, player);
		NpcHtmlMessagePacket itemReply = new NpcHtmlMessagePacket(null, 0, htmlContent);
		player.sendPacket(itemReply);
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		return true;
	}
}