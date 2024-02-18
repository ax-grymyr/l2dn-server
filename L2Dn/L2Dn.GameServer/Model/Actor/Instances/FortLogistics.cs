using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
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
			HtmlPacketHelper helper;
			if (isMyLord)
			{
				helper = new HtmlPacketHelper(DataFileLocation.Data, "data/html/fortress/logistics-rewards.htm");
				helper.Replace("%bloodoath%", player.getClan().getBloodOathCount().ToString());
			}
			else
			{
				helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-noprivs.htm");
			}
			helper.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId());
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("blood"))
		{
			HtmlPacketHelper helper;
			if (isMyLord)
			{
				int blood = player.getClan().getBloodOathCount();
				if (blood > 0)
				{
					player.addItem("Quest", 9910, blood, this, true);
					player.getClan().resetBloodOathCount();
					helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-blood.htm");
				}
				else
				{
					helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-noblood.htm");
				}
			}
			else
			{
				helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-noprivs.htm");
			}
			
			helper.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("supplylvl"))
		{
			HtmlPacketHelper helper;
			if (getFort().getFortState() == 2)
			{
				if (player.isClanLeader())
				{
					helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-supplylvl.htm");
					helper.Replace("%supplylvl%", getFort().getSupplyLvL().ToString());
				}
				else
				{
					helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-noprivs.htm");
				}
			}
			else
			{
				helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-1.htm"); // TODO: Missing HTML?
			}
			
			helper.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
			player.sendPacket(html);
		}
		else if (actualCommand.equalsIgnoreCase("supply"))
		{
			HtmlPacketHelper helper;
			if (isMyLord)
			{
				if (getFort().getSiege().isInProgress())
				{
					helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-siege.htm");
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
						
						helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-supply.htm");
					}
					else
					{
						helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-nosupply.htm");
					}
				}
			}
			else
			{
				helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/logistics-noprivs.htm");
			}
			
			helper.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
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
		
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, filename);
		helper.Replace("%objectId%", getObjectId().ToString());
		helper.Replace("%npcId%", getId().ToString());
		if (getFort().getOwnerClan() != null)
		{
			helper.Replace("%clanname%", getFort().getOwnerClan().getName());
		}
		else
		{
			helper.Replace("%clanname%", "NPC");
		}

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
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