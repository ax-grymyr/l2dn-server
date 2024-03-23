using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ActionShiftHandlers;

/**
 * This class manage shift + click on {@link Door}.
 * @author St3eT
 */
public class DoorActionShift: IActionShiftHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		if (player.isGM())
		{
			player.setTarget(target);
			Door door = (Door) target;
			ClanHall clanHall = ClanHallData.getInstance().getClanHallByDoorId(door.getId());
			Fort fort = door.getFort();
			Castle castle = door.getCastle();
			player.sendPacket(new StaticObjectInfoPacket(door, player.isGM()));

			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/doorinfo.htm", player);

			// Hp / MP
			htmlContent.Replace("%hpGauge%", HtmlUtil.getHpGauge(250, (long) door.getCurrentHp(), door.getMaxHp(), false));
			htmlContent.Replace("%mpGauge%", HtmlUtil.getMpGauge(250, (long) door.getCurrentMp(), door.getMaxMp(), false));
			// Basic info
			htmlContent.Replace("%doorName%", door.getName());
			htmlContent.Replace("%objId%", door.getObjectId().ToString());
			htmlContent.Replace("%doorId%", door.getId().ToString());
			// Position info
			htmlContent.Replace("%position%", door.getX() + ", " + door.getY() + ", " + door.getZ());
			htmlContent.Replace("%node1%", door.getX(0) + ", " + door.getY(0) + ", " + door.getZMin());
			htmlContent.Replace("%node2%", door.getX(1) + ", " + door.getY(1) + ", " + door.getZMin());
			htmlContent.Replace("%node3%", door.getX(2) + ", " + door.getY(2) + ", " + door.getZMax());
			htmlContent.Replace("%node4%", door.getX(3) + ", " + door.getY(3) + ", " + door.getZMax());
			// Residence info
			htmlContent.Replace("%fortress%", fort != null ? fort.getName() : "None");
			htmlContent.Replace("%clanHall%", clanHall != null ? clanHall.getName() : "None");
			htmlContent.Replace("%castle%", castle != null ? castle.getName() + " Castle" : "None");

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
			player.sendPacket(html);
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Door;
	}
}