using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.ActionShiftHandlers;

public class StaticObjectActionShift: IActionShiftHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		if (player.isGM())
		{
			player.setTarget(target);
			player.sendPacket(new StaticObjectInfoPacket((StaticObject) target));

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1,
				"<html><body><center><font color=\"LEVEL\">Static Object Info</font></center><br><table border=0><tr><td>Coords X,Y,Z: </td><td>" +
				target.getX() + ", " + target.getY() + ", " + target.getZ() + "</td></tr><tr><td>Object ID: </td><td>" +
				target.getObjectId() + "</td></tr><tr><td>Static Object ID: </td><td>" + target.getId() +
				"</td></tr><tr><td>Mesh Index: </td><td>" + ((StaticObject)target).getMeshIndex() +
				"</td></tr><tr><td><br></td></tr><tr><td>Class: </td><td>" + target.GetType().Name +
				"</td></tr></table></body></html>");
			
			player.sendPacket(html);
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.StaticObject;
	}
}