using L2Dn.GameServer.Data;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.ItemHandlers;

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
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/item/" + itemId + ".htm");
		helper.Replace("%itemId%", item.getObjectId().ToString());
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, item.getId(), helper);
		player.sendPacket(html);
		
		return true;
	}
}