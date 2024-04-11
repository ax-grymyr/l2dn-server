using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FortLogistics : Merchant
{
	private static readonly int[] SUPPLY_BOX_IDS =
	[
		35665, 35697, 35734, 35766, 35803, 35834, 35866, 35903, 35935, 35973, 36010, 36042, 36080, 36117, 36148, 36180,
		36218, 36256, 36293, 36325, 36363
	];
	
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

		if (actualCommand.equalsIgnoreCase("rewards"))
		{
			HtmlContent htmlContent;
			if (isMyLord)
			{
				htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-rewards.htm", player);
				htmlContent.Replace("%bloodoath%", player.getClan().getBloodOathCount().ToString());
			}
			else
			{
				htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-noprivs.htm", player);
			}
			htmlContent.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), 0, htmlContent);
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("blood"))
		{
			HtmlContent htmlContent;
			if (isMyLord)
			{
				int blood = player.getClan().getBloodOathCount();
				if (blood > 0)
				{
					player.addItem("Quest", 9910, blood, this, true);
					player.getClan().resetBloodOathCount();
					htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-blood.htm", player);
				}
				else
				{
					htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-noblood.htm", player);
				}
			}
			else
			{
				htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-noprivs.htm", player);
			}
			
			htmlContent.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), 0, htmlContent);
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("supplylvl"))
		{
			HtmlContent htmlContent;
			if (getFort().getFortState() == 2)
			{
				if (player.isClanLeader())
				{
					htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-supplylvl.htm", player);
					htmlContent.Replace("%supplylvl%", getFort().getSupplyLevel().ToString());
				}
				else
				{
					htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-noprivs.htm", player);
				}
			}
			else
			{
				htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-1.htm", player); // TODO: Missing HTML?
			}
			
			htmlContent.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), 0, htmlContent);
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("supply"))
		{
			HtmlContent htmlContent;
			if (isMyLord)
			{
				if (getFort().getSiege().isInProgress())
				{
					htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-siege.htm", player);
				}
				else
				{
					int level = getFort().getSupplyLevel();
					if (level > 0)
					{
						// spawn box
						NpcTemplate boxTemplate = NpcData.getInstance().getTemplate(SUPPLY_BOX_IDS[level - 1]);
						Monster box = new Monster(boxTemplate);
						box.setCurrentHp(box.getMaxHp());
						box.setCurrentMp(box.getMaxMp());
						box.setHeading(0);
						box.spawnMe(getX() - 23, getY() + 41, getZ());
						getFort().setSupplyLevel(0);
						getFort().saveFortVariables();
						
						htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-supply.htm", player);
					}
					else
					{
						htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-nosupply.htm", player);
					}
				}
			}
			else
			{
				htmlContent = HtmlContent.LoadFromFile("html/fortress/logistics-noprivs.htm", player);
			}
			
			htmlContent.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), 0, htmlContent);
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
			filename = "html/fortress/logistics.htm";
		}
		else
		{
			filename = "html/fortress/logistics-" + value + ".htm";
		}
		
		HtmlContent htmlContent = HtmlContent.LoadFromFile(filename, player);
		htmlContent.Replace("%objectId%", getObjectId().ToString());
		htmlContent.Replace("%npcId%", getId().ToString());
		if (getFort().getOwnerClan() != null)
		{
			htmlContent.Replace("%clanname%", getFort().getOwnerClan().getName());
		}
		else
		{
			htmlContent.Replace("%clanname%", "NPC");
		}

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), 0, htmlContent);
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
		return "html/fortress/" + pom + ".htm";
	}
	
	public override bool hasRandomAnimation()
	{
		return false;
	}
}