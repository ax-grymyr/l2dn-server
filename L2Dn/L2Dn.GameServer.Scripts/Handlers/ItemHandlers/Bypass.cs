using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author JIV
 */
public class Bypass: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			return false;
		}
		
		Player player = (Player)playable;
		int itemId = item.getId();
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/item/" + itemId + ".htm", player);
		htmlContent.Replace("%itemId%", item.ObjectId.ToString());
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, item.getId(), htmlContent);
		player.sendPacket(html);
		
		return true;
	}
}