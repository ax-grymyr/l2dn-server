using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.InstanceManagers.Games;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class RaceManager: Npc
{
	protected static readonly int[] TICKET_PRICES = [100, 500, 1000, 5000, 10000, 20000, 50000, 100000];

	public RaceManager(NpcTemplate template): base(template)
	{
	}

	public override void onBypassFeedback(Player player, string command)
	{
		if (command.startsWith("BuyTicket"))
		{
			if (!Config.General.ALLOW_RACE || MonsterRace.getInstance().getCurrentRaceState() != MonsterRace.RaceState.ACCEPTING_BETS)
			{
				player.sendPacket(SystemMessageId.MONSTER_RACE_TICKETS_ARE_NO_LONGER_AVAILABLE);
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			int val = int.Parse(command.Substring(10));
			if (val == 0)
			{
				player.setRaceTicket(0, 0);
				player.setRaceTicket(1, 0);
			}

			if ((val == 10 && player.getRaceTicket(0) == 0) || (val == 20 && player.getRaceTicket(0) == 0 && player.getRaceTicket(1) == 0))
			{
				val = 0;
			}

			string search, replace;

			HtmlContent htmlContent;
			if (val < 10)
			{
				htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 2, player), player);
				for (int i = 0; i < 8; i++)
				{
					int n = i + 1;
					search = "Mob" + n;
					htmlContent.Replace(search, MonsterRace.getInstance().getMonsters()[i].getTemplate().getName());
				}
				search = "No1";
				if (val == 0)
				{
					htmlContent.Replace(search, "");
				}
				else
				{
					htmlContent.Replace(search, val.ToString());
					player.setRaceTicket(0, val);
				}
			}
			else if (val < 20)
			{
				if (player.getRaceTicket(0) == 0)
				{
					return;
				}

				htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 3, player), player);
				htmlContent.Replace("0place", player.getRaceTicket(0).ToString());
				search = "Mob1";
				replace = MonsterRace.getInstance().getMonsters()[player.getRaceTicket(0) - 1].getTemplate().getName();
				htmlContent.Replace(search, replace);
				search = "0adena";
				if (val == 10)
				{
					htmlContent.Replace(search, "");
				}
				else
				{
					htmlContent.Replace(search, TICKET_PRICES[val - 11].ToString());
					player.setRaceTicket(1, val - 10);
				}
			}
			else if (val == 20)
			{
				if (player.getRaceTicket(0) == 0 || player.getRaceTicket(1) == 0)
				{
					return;
				}

				htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 4, player), player);
				htmlContent.Replace("0place", player.getRaceTicket(0).ToString());
				search = "Mob1";
				replace = MonsterRace.getInstance().getMonsters()[player.getRaceTicket(0) - 1].getTemplate().getName();
				htmlContent.Replace(search, replace);
				search = "0adena";
				int price = TICKET_PRICES[player.getRaceTicket(1) - 1];
				htmlContent.Replace(search, price.ToString());
				search = "0tax";
				int tax = 0;
				htmlContent.Replace(search, tax.ToString());
				search = "0total";
				int total = price + tax;
				htmlContent.Replace(search, total.ToString());
			}
			else
			{
				if (player.getRaceTicket(0) == 0 || player.getRaceTicket(1) == 0)
				{
					return;
				}

				int ticket = player.getRaceTicket(0);
				int priceId = player.getRaceTicket(1);
				if (!player.reduceAdena("Race", TICKET_PRICES[priceId - 1], this, true))
				{
					return;
				}

				player.setRaceTicket(0, 0);
				player.setRaceTicket(1, 0);
				Item item = new Item(IdManager.getInstance().getNextId(), 4443);
				item.setCount(1);
				item.setEnchantLevel(MonsterRace.getInstance().getRaceNumber());
				item.setCustomType1(ticket);
				item.setCustomType2(TICKET_PRICES[priceId - 1] / 100);
				player.addItem("Race", item, player, false);
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2_2);
				msg.Params.addInt(MonsterRace.getInstance().getRaceNumber());
				msg.Params.addItemName(4443);
				player.sendPacket(msg);

				// Refresh lane bet.
				MonsterRace.getInstance().setBetOnLane(ticket, TICKET_PRICES[priceId - 1], true);
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			htmlContent.Replace("1race", MonsterRace.getInstance().getRaceNumber().ToString());
			htmlContent.Replace("%objectId%", ObjectId.ToString());

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
			player.sendPacket(html);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		else if (command.equals("ShowOdds"))
		{
			if (!Config.General.ALLOW_RACE || MonsterRace.getInstance().getCurrentRaceState() == MonsterRace.RaceState.ACCEPTING_BETS)
			{
				player.sendPacket(SystemMessageId.MONSTER_RACE_PAYOUT_INFORMATION_IS_NOT_AVAILABLE_WHILE_TICKETS_ARE_BEING_SOLD);
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			HtmlContent htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 5, player), player);
			for (int i = 0; i < 8; i++)
			{
				int n = i + 1;
				htmlContent.Replace("Mob" + n, MonsterRace.getInstance().getMonsters()[i].getTemplate().getName());

				// Odd
				double odd = MonsterRace.getInstance().getOdds()[i];
				htmlContent.Replace("Odd" + n, odd > 0D ? odd.ToString("N1") : "&$804;");
			}

			htmlContent.Replace("1race", MonsterRace.getInstance().getRaceNumber().ToString());
			htmlContent.Replace("%objectId%", ObjectId.ToString());

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
			player.sendPacket(html);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		else if (command.equals("ShowInfo"))
		{
			if (!Config.General.ALLOW_RACE)
			{
				return;
			}

			HtmlContent htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 6, player), player);
			for (int i = 0; i < 8; i++)
			{
				int n = i + 1;
				string search = "Mob" + n;
				htmlContent.Replace(search, MonsterRace.getInstance().getMonsters()[i].getTemplate().getName());
			}

			htmlContent.Replace("%objectId%", ObjectId.ToString());

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
			player.sendPacket(html);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		else if (command.equals("ShowTickets"))
		{
			if (!Config.General.ALLOW_RACE)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			// Generate data.
			StringBuilder sb = new();

			// Retrieve player's tickets.
			foreach (Item ticket in player.getInventory().getAllItemsByItemId(4443))
			{
				// Don't list current race tickets.
				if (ticket.getEnchantLevel() == MonsterRace.getInstance().getRaceNumber())
				{
					continue;
				}

				sb.Append("<tr><td><a action=\"bypass -h npc_%objectId%_ShowTicket ");
				sb.Append(ticket.ObjectId);
				sb.Append("\">");
				sb.Append(ticket.getEnchantLevel());
				sb.Append(" Race Number</a></td><td align=right><font color=\"LEVEL\">");
				sb.Append(ticket.getCustomType1());
				sb.Append("</font> Number</td><td align=right><font color=\"LEVEL\">");
				sb.Append(ticket.getCustomType2() * 100);
				sb.Append("</font> Adena</td></tr>");
			}

			HtmlContent htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 7, player), player);
			htmlContent.Replace("%tickets%", sb.ToString());
			htmlContent.Replace("%objectId%", ObjectId.ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
			player.sendPacket(html);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		else if (command.startsWith("ShowTicket"))
		{
			// Retrieve ticket objectId.
			int val = int.Parse(command.Substring(11));
			if (!Config.General.ALLOW_RACE || val == 0)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			// Retrieve ticket on player's inventory.
			Item? ticket = player.getInventory().getItemByObjectId(val);
			if (ticket == null)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			int raceId = ticket.getEnchantLevel();
			int lane = ticket.getCustomType1();
			int bet = ticket.getCustomType2() * 100;

			// Retrieve HistoryInfo for that race.
			MonsterRace.HistoryInfo info = MonsterRace.getInstance().getHistory()[raceId - 1];
			if (info == null)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			HtmlContent htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 8, player), player);
			htmlContent.Replace("%raceId%", raceId.ToString());
			htmlContent.Replace("%lane%", lane.ToString());
			htmlContent.Replace("%bet%", bet.ToString());
			htmlContent.Replace("%firstLane%", info.getFirst().ToString());
			htmlContent.Replace("%odd%", lane == info.getFirst() ? info.getOddRate().ToString("N2") : "0.01");
			htmlContent.Replace("%objectId%", ObjectId.ToString());
			htmlContent.Replace("%ticketObjectId%", val.ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
			player.sendPacket(html);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		else if (command.startsWith("CalculateWin"))
		{
			// Retrieve ticket objectId.
			int val = int.Parse(command.Substring(13));
			if (!Config.General.ALLOW_RACE || val == 0)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			// Delete ticket on player's inventory.
			Item? ticket = player.getInventory().getItemByObjectId(val);
			if (ticket == null)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			int raceId = ticket.getEnchantLevel();
			int lane = ticket.getCustomType1();
			int bet = ticket.getCustomType2() * 100;

			// Retrieve HistoryInfo for that race.
			MonsterRace.HistoryInfo info = MonsterRace.getInstance().getHistory()[raceId - 1];
			if (info == null)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			// Destroy the ticket.
			if (player.destroyItem("MonsterTrack", ticket, this, true))
			{
				player.addAdena("MonsterTrack", (int) (bet * (lane == info.getFirst() ? info.getOddRate() : 0.01)), this, true);
			}

			base.onBypassFeedback(player, "Chat 0");
			return;
		}
		else if (command.equals("ViewHistory"))
		{
			if (!Config.General.ALLOW_RACE)
			{
				base.onBypassFeedback(player, "Chat 0");
				return;
			}

			// Generate data.
			StringBuilder sb = new StringBuilder();

			// Use whole history, pickup from 'last element' and stop at 'latest element - 7'.
			List<MonsterRace.HistoryInfo> history = MonsterRace.getInstance().getHistory();
			for (int i = history.Count - 1; i >= Math.Max(0, history.Count - 7); i--)
			{
				MonsterRace.HistoryInfo info = history[i];

				sb.Append("<tr><td><font color=\"LEVEL\">");
				sb.Append(info.getRaceId());
				sb.Append("</font> th</td><td><font color=\"LEVEL\">");
				sb.Append(info.getFirst());
				sb.Append("</font> Lane </td><td><font color=\"LEVEL\">");
				sb.Append(info.getSecond());
				sb.Append("</font> Lane</td><td align=right><font color=00ffff>");
				sb.Append(info.getOddRate().ToString("N2"));
				sb.Append("</font> Times</td></tr>");
			}

			HtmlContent htmlContent = HtmlContent.LoadFromFile(getHtmlPath(Id, 9, player), player);
			htmlContent.Replace("%infos%", sb.ToString());
			htmlContent.Replace("%objectId%", ObjectId.ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
			player.sendPacket(html);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		else
		{
			base.onBypassFeedback(player, command);
		}
	}
}