using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Teleporters;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Doorman : Folk
{
	public Doorman(NpcTemplate template): base(template)
	{
		setInstanceType(InstanceType.Doorman);
	}
	
	public override bool isAutoAttackable(Creature attacker)
	{
		if (attacker.isMonster())
		{
			return true;
		}
		return base.isAutoAttackable(attacker);
	}
	
	public override void onBypassFeedback(Player player, String command)
	{
		if (command.startsWith("Chat"))
		{
			showChatWindow(player);
			return;
		}
		else if (command.startsWith("open_doors"))
		{
			if (isOwnerClan(player))
			{
				if (isUnderSiege())
				{
					cannotManageDoors(player);
				}
				else
				{
					openDoors(player, command);
				}
			}
			return;
		}
		else if (command.startsWith("close_doors"))
		{
			if (isOwnerClan(player))
			{
				if (isUnderSiege())
				{
					cannotManageDoors(player);
				}
				else
				{
					closeDoors(player, command);
				}
			}
			return;
		}
		else if (command.startsWith("tele"))
		{
			if (isOwnerClan(player))
			{
				TeleportHolder holder = TeleporterData.getInstance().getHolder(getId(), TeleportType.OTHER.ToString());
				if (holder != null)
				{
					int locId = int.Parse(command.Substring(5).Trim());
					holder.doTeleport(player, this, locId);
				}
			}
			return;
		}
		base.onBypassFeedback(player, command);
	}
	
	public override void showChatWindow(Player player)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);

		HtmlPacketHelper helper;
		if (!isOwnerClan(player))
		{
			helper = new HtmlPacketHelper(DataFileLocation.Data, "html/doorman/" + getTemplate().getId() + "-no.htm");
		}
		else
		{
			helper = new HtmlPacketHelper(DataFileLocation.Data, "html/doorman/" + getTemplate().getId() + ".htm");
		}
		
		helper.Replace("%objectId%", getObjectId().ToString());

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
		player.sendPacket(html);
	}
	
	protected virtual void openDoors(Player player, String command)
	{
		 StringTokenizer st = new StringTokenizer(command.Substring(10), ", ");
		st.nextToken();
		
		while (st.hasMoreTokens())
		{
			DoorData.getInstance().getDoor(int.Parse(st.nextToken())).openMe();
		}
	}
	
	protected virtual void closeDoors(Player player, String command)
	{
		 StringTokenizer st = new StringTokenizer(command.Substring(11), ", ");
		st.nextToken();
		
		while (st.hasMoreTokens())
		{
			DoorData.getInstance().getDoor(int.Parse(st.nextToken())).closeMe();
		}
	}
	
	protected void cannotManageDoors(Player player)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/doorman/" + getTemplate().getId() + "-busy.htm");
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
		player.sendPacket(html);
	}
	
	protected virtual bool isOwnerClan(Player player)
	{
		return true;
	}
	
	protected virtual bool isUnderSiege()
	{
		return false;
	}
}