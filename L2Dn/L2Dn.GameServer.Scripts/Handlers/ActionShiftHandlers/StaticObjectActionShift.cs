using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ActionShiftHandlers;

public class StaticObjectActionShift: IActionShiftHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		if (player.isGM())
		{
			player.setTarget(target);
			player.sendPacket(new StaticObjectInfoPacket((StaticObject) target));

			string html =
				"<html><body><center><font color=\"LEVEL\">Static Object Info</font></center><br><table border=0><tr><td>Coords X,Y,Z: </td><td>" +
				target.getX() + ", " + target.getY() + ", " + target.getZ() + "</td></tr><tr><td>Object ID: </td><td>" +
				target.ObjectId + "</td></tr><tr><td>Static Object ID: </td><td>" + target.Id +
				"</td></tr><tr><td>Mesh Index: </td><td>" + ((StaticObject)target).getMeshIndex() +
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
		return InstanceType.StaticObject;
	}
}