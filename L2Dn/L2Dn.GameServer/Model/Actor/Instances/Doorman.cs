using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Teleporters;
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
				TeleportHolder holder = TeleporterData.getInstance().getHolder(getId(), TeleportType.OTHER.name());
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
		
		 NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
		if (!isOwnerClan(player))
		{
			html.setFile(player, "data/html/doorman/" + getTemplate().getId() + "-no.htm");
		}
		else
		{
			html.setFile(player, "data/html/doorman/" + getTemplate().getId() + ".htm");
		}
		
		html.replace("%objectId%", String.valueOf(getObjectId()));
		player.sendPacket(html);
	}
	
	protected void openDoors(Player player, String command)
	{
		 StringTokenizer st = new StringTokenizer(command.Substring(10), ", ");
		st.nextToken();
		
		while (st.hasMoreTokens())
		{
			DoorData.getInstance().getDoor(int.Parse(st.nextToken())).openMe();
		}
	}
	
	protected void closeDoors(Player player, String command)
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
		
		 NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
		html.setFile(player, "data/html/doorman/" + getTemplate().getId() + "-busy.htm");
		player.sendPacket(html);
	}
	
	protected bool isOwnerClan(Player player)
	{
		return true;
	}
	
	protected bool isUnderSiege()
	{
		return false;
	}
}
