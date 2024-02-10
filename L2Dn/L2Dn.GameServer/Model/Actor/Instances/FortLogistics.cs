using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FortLogistics : Merchant
{
	private static readonly int[] SUPPLY_BOX_IDS =
	{
		35665,
		35697,
		35734,
		35766,
		35803,
		35834,
		35866,
		35903,
		35935,
		35973,
		36010,
		36042,
		36080,
		36117,
		36148,
		36180,
		36218,
		36256,
		36293,
		36325,
		36363
	};
	
	public FortLogistics(NpcTemplate template): base(template)
	{
		setInstanceType(InstanceType.FortLogistics);
	}
	
	public override void onBypassFeedback(Player player, String command)
	{
		if (player.getLastFolkNPC().getObjectId() != getObjectId())
		{
			return;
		}
		
		StringTokenizer st = new StringTokenizer(command, " ");
		String actualCommand = st.nextToken(); // Get actual command
		bool isMyLord = player.isClanLeader() && (player.getClan().getFortId() == (getFort() != null ? getFort().getResidenceId() : -1));
		NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
		if (actualCommand.equalsIgnoreCase("rewards"))
		{
			if (isMyLord)
			{
				html.setFile(player, "data/html/fortress/logistics-rewards.htm");
				html.replace("%bloodoath%", String.valueOf(player.getClan().getBloodOathCount()));
			}
			else
			{
				html.setFile(player, "data/html/fortress/logistics-noprivs.htm");
			}
			html.replace("%objectId%", String.valueOf(getObjectId()));
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("blood"))
		{
			if (isMyLord)
			{
				int blood = player.getClan().getBloodOathCount();
				if (blood > 0)
				{
					player.addItem("Quest", 9910, blood, this, true);
					player.getClan().resetBloodOathCount();
					html.setFile(player, "data/html/fortress/logistics-blood.htm");
				}
				else
				{
					html.setFile(player, "data/html/fortress/logistics-noblood.htm");
				}
			}
			else
			{
				html.setFile(player, "data/html/fortress/logistics-noprivs.htm");
			}
			html.replace("%objectId%", String.valueOf(getObjectId()));
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("supplylvl"))
		{
			if (getFort().getFortState() == 2)
			{
				if (player.isClanLeader())
				{
					html.setFile(player, "data/html/fortress/logistics-supplylvl.htm");
					html.replace("%supplylvl%", String.valueOf(getFort().getSupplyLvL()));
				}
				else
				{
					html.setFile(player, "data/html/fortress/logistics-noprivs.htm");
				}
			}
			else
			{
				html.setFile(player, "data/html/fortress/logistics-1.htm"); // TODO: Missing HTML?
			}
			html.replace("%objectId%", String.valueOf(getObjectId()));
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("supply"))
		{
			if (isMyLord)
			{
				if (getFort().getSiege().isInProgress())
				{
					html.setFile(player, "data/html/fortress/logistics-siege.htm");
				}
				else
				{
					int level = getFort().getSupplyLvL();
					if (level > 0)
					{
						// spawn box
						NpcTemplate boxTemplate = NpcData.getInstance().getTemplate(SUPPLY_BOX_IDS[level - 1]);
						Monster box = new Monster(boxTemplate);
						box.setCurrentHp(box.getMaxHp());
						box.setCurrentMp(box.getMaxMp());
						box.setHeading(0);
						box.spawnMe(getX() - 23, getY() + 41, getZ());
						getFort().setSupplyLvL(0);
						getFort().saveFortVariables();
						
						html.setFile(player, "data/html/fortress/logistics-supply.htm");
					}
					else
					{
						html.setFile(player, "data/html/fortress/logistics-nosupply.htm");
					}
				}
			}
			else
			{
				html.setFile(player, "data/html/fortress/logistics-noprivs.htm");
			}
			html.replace("%objectId%", String.valueOf(getObjectId()));
			player.sendPacket(html);
		}
		else
		{
			base.onBypassFeedback(player, command);
		}
	}
	
	public override void showChatWindow(Player player)
	{
		showMessageWindow(player, 0);
	}
	
	private void showMessageWindow(Player player, int value)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		
		String filename;
		if (value == 0)
		{
			filename = "data/html/fortress/logistics.htm";
		}
		else
		{
			filename = "data/html/fortress/logistics-" + value + ".htm";
		}
		
		NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
		html.setFile(player, filename);
		html.replace("%objectId%", String.valueOf(getObjectId()));
		html.replace("%npcId%", String.valueOf(getId()));
		if (getFort().getOwnerClan() != null)
		{
			html.replace("%clanname%", getFort().getOwnerClan().getName());
		}
		else
		{
			html.replace("%clanname%", "NPC");
		}
		player.sendPacket(html);
	}
	
	public override String getHtmlPath(int npcId, int value, Player player)
	{
		String pom = "";
		if (value == 0)
		{
			pom = "logistics";
		}
		else
		{
			pom = "logistics-" + value;
		}
		return "data/html/fortress/" + pom + ".htm";
	}
	
	public override bool hasRandomAnimation()
	{
		return false;
	}
}