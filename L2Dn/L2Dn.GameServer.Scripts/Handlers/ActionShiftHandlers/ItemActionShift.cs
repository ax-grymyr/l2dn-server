using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ActionShiftHandlers;

public class ItemActionShift: IActionShiftHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		if (player.isGM())
		{
			string html =
				"<html><head><title>" + target.getName() +
				"</title></head><body><center><font color=\"LEVEL\">Item Info</font></center><br><table border=0><tr><td>Object ID: </td><td>" +
				target.getObjectId() + "</td></tr><tr><td>Item ID: </td><td>" + target.getId() +
				"</td></tr><tr><td>Owner ID: </td><td>" + ((Item)target).getOwnerId() +
				"</td></tr><tr><td>Location: </td><td>" + target.Location +
				"</td></tr><tr><td><br></td></tr><tr><td>Class: </td><td>" + target.GetType().Name +
				"</td></tr></table></body></html>";

			HtmlContent htmlContent = HtmlContent.LoadFromText(html, player);
			NpcHtmlMessagePacket htmlPacket = new NpcHtmlMessagePacket(null, 1, htmlContent);
			player.sendPacket(htmlPacket);
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Item;
	}
}