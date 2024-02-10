using System.Globalization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class PetManager: Merchant
{
	public PetManager(NpcTemplate template): base(template)
	{
		setInstanceType(InstanceType.PetManager);
	}

	public override String getHtmlPath(int npcId, int value, Player player)
	{
		String pom = "";
		if (value == 0)
		{
			pom = npcId.ToString(CultureInfo.InvariantCulture);
		}
		else
		{
			pom = npcId + "-" + value;
		}

		return "data/html/petmanager/" + pom + ".htm";
	}

	public override void showChatWindow(Player player)
	{
		String filename = "data/html/petmanager/" + getId() + ".htm";
		if ((getId() == 36478) && player.hasSummon())
		{
			filename = "data/html/petmanager/restore-unsummonpet.htm";
		}

		NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
		html.setFile(player, filename);
		html.replace("%objectId%", String.valueOf(getObjectId()));
		html.replace("%npcname%", getName());
		player.sendPacket(html);
	}

	public override void onBypassFeedback(Player player, String command)
	{
		if (command.startsWith("exchange"))
		{
			String[] @params = command.Split(" ");
			int val = Integer.parseInt(@params[1]);
			switch (val)
			{
				case 1:
				{
					exchange(player, 7585, 6650);
					break;
				}
				case 2:
				{
					exchange(player, 7583, 6648);
					break;
				}
				case 3:
				{
					exchange(player, 7584, 6649);
					break;
				}
			}
		}
		else if (command.startsWith("evolve"))
		{
			String[] @params = command.Split(" ");
			int val = Integer.parseInt(@params[1]);
			bool ok = false;
			switch (val)
			{
				// Info evolve(player, "current pet summon item", "new pet summon item", "level required to evolve")
				// To ignore evolve just put value 0 where do you like example: evolve(player, 0, 9882, 55);
				case 1:
				{
					ok = Evolve.doEvolve(player, this, 2375, 9882, 55);
					break;
				}
				case 2:
				{
					ok = Evolve.doEvolve(player, this, 9882, 10426, 70);
					break;
				}
				case 3:
				{
					ok = Evolve.doEvolve(player, this, 6648, 10311, 55);
					break;
				}
				case 4:
				{
					ok = Evolve.doEvolve(player, this, 6650, 10313, 55);
					break;
				}
				case 5:
				{
					ok = Evolve.doEvolve(player, this, 6649, 10312, 55);
					break;
				}
			}

			if (!ok)
			{
				NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
				html.setFile(player, "data/html/petmanager/evolve_no.htm");
				player.sendPacket(html);
			}
		}
		else if (command.startsWith("restore"))
		{
			String[] @params = command.Split(" ");
			int val = Integer.parseInt(@params[1]);
			bool ok = false;
			switch (val)
			{
				// Info evolve(player, "curent pet summon item", "new pet summon item", "level required to evolve")
				case 1:
				{
					ok = Evolve.doRestore(player, this, 10307, 9882, 55);
					break;
				}
				case 2:
				{
					ok = Evolve.doRestore(player, this, 10611, 10426, 70);
					break;
				}
				case 3:
				{
					ok = Evolve.doRestore(player, this, 10308, 4422, 55);
					break;
				}
				case 4:
				{
					ok = Evolve.doRestore(player, this, 10309, 4423, 55);
					break;
				}
				case 5:
				{
					ok = Evolve.doRestore(player, this, 10310, 4424, 55);
					break;
				}
			}

			if (!ok)
			{
				NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
				html.setFile(player, "data/html/petmanager/restore_no.htm");
				player.sendPacket(html);
			}
		}
		else
		{
			base.onBypassFeedback(player, command);
		}
	}

	public void exchange(Player player, int itemIdtake, int itemIdgive)
	{
		NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
		if (player.destroyItemByItemId("Consume", itemIdtake, 1, this, true))
		{
			player.addItem("", itemIdgive, 1, this, true);
			html.setFile(player, "data/html/petmanager/" + getId() + ".htm");
			player.sendPacket(html);
		}
		else
		{
			html.setFile(player, "data/html/petmanager/exchange_no.htm");
			player.sendPacket(html);
		}
	}
}