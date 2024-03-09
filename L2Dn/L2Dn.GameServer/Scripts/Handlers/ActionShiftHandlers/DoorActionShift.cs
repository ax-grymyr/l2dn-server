using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
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

			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/admin/doorinfo.htm");

			// Hp / MP
			helper.Replace("%hpGauge%", HtmlUtil.getHpGauge(250, (long) door.getCurrentHp(), door.getMaxHp(), false));
			helper.Replace("%mpGauge%", HtmlUtil.getMpGauge(250, (long) door.getCurrentMp(), door.getMaxMp(), false));
			// Basic info
			helper.Replace("%doorName%", door.getName());
			helper.Replace("%objId%", door.getObjectId().ToString());
			helper.Replace("%doorId%", door.getId().ToString());
			// Position info
			helper.Replace("%position%", door.getX() + ", " + door.getY() + ", " + door.getZ());
			helper.Replace("%node1%", door.getX(0) + ", " + door.getY(0) + ", " + door.getZMin());
			helper.Replace("%node2%", door.getX(1) + ", " + door.getY(1) + ", " + door.getZMin());
			helper.Replace("%node3%", door.getX(2) + ", " + door.getY(2) + ", " + door.getZMax());
			helper.Replace("%node4%", door.getX(3) + ", " + door.getY(3) + ", " + door.getZMax());
			// Residence info
			helper.Replace("%fortress%", fort != null ? fort.getName() : "None");
			helper.Replace("%clanHall%", clanHall != null ? clanHall.getName() : "None");
			helper.Replace("%castle%", castle != null ? castle.getName() + " Castle" : "None");

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, helper);
			player.sendPacket(html);
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Door;
	}
}