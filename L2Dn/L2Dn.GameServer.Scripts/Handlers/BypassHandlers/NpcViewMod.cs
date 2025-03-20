using System.Text;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author NosBit
 */
public class NpcViewMod: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(NpcViewMod));

    private static readonly string[] COMMANDS = ["NpcViewMod"];

	private const int DROP_LIST_ITEMS_PER_PAGE = 10;

	public bool useBypass(string command, Player player, Creature? bypassOrigin)
	{
		StringTokenizer st = new StringTokenizer(command);
		st.nextToken();

		if (!st.hasMoreTokens())
		{
			_logger.Warn("Bypass[NpcViewMod] used without enough parameters.");
			return false;
		}

		string actualCommand = st.nextToken();
		switch (actualCommand.toLowerCase())
		{
			case "view":
			{
				WorldObject? target;
				if (st.hasMoreElements())
				{
					try
					{
						target = World.getInstance().findObject(int.Parse(st.nextToken()));
					}
					catch (FormatException e)
					{
                        _logger.Error(e);
						return false;
					}
				}
				else
				{
					target = player.getTarget();
				}

				Npc? npc = target is Npc ? (Npc) target : null;
				if (npc == null)
				{
					return false;
				}

				sendNpcView(player, npc);
				break;
			}
			case "droplist":
			{
				if (st.countTokens() < 2)
				{
					_logger.Warn("Bypass[NpcViewMod] used without enough parameters.");
					return false;
				}

				string dropListTypeString = st.nextToken();
				try
				{
					DropType dropListType = Enum.Parse<DropType>(dropListTypeString);
					WorldObject? target = World.getInstance().findObject(int.Parse(st.nextToken()));
					Npc? npc = target is Npc ? (Npc) target : null;
					if (npc == null)
					{
						return false;
					}
					int page = st.hasMoreElements() ? int.Parse(st.nextToken()) : 0;
					sendNpcDropList(player, npc, dropListType, page);
				}
				catch (FormatException e)
				{
                    _logger.Error(e);
					return false;
				}
				catch (ArgumentException e)
                {
                    _logger.Warn($"Bypass[NpcViewMod] unknown drop list scope: {dropListTypeString}, error: {e}");
					return false;
				}
				break;
			}
			case "skills":
			{
				WorldObject? target;
				if (st.hasMoreElements())
				{
					try
					{
						target = World.getInstance().findObject(int.Parse(st.nextToken()));
					}
					catch (FormatException e)
					{
                        _logger.Error(e);
						return false;
					}
				}
				else
				{
					target = player.getTarget();
				}

				Npc? npc = target is Npc ? (Npc) target : null;
				if (npc == null)
				{
					return false;
				}

				sendNpcSkillView(player, npc);
				break;
			}
			case "aggrolist":
			{
				WorldObject? target;
				if (st.hasMoreElements())
				{
					try
					{
						target = World.getInstance().findObject(int.Parse(st.nextToken()));
					}
					catch (FormatException e)
					{
                        _logger.Error(e);
						return false;
					}
				}
				else
				{
					target = player.getTarget();
				}

				Npc? npc = target is Npc ? (Npc) target : null;
				if (npc == null)
				{
					return false;
				}

				sendAggroListView(player, npc);
				break;
			}
		}

		return true;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}

	public static void sendNpcView(Player player, Npc npc)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/NpcView/Info.htm", player);
		htmlContent.Replace("%name%", npc.getName());
		htmlContent.Replace("%hpGauge%", HtmlUtil.getHpGauge(250, (long) npc.getCurrentHp(), npc.getMaxHp(), false));
		htmlContent.Replace("%mpGauge%", HtmlUtil.getMpGauge(250, (long) npc.getCurrentMp(), npc.getMaxMp(), false));

		Spawn? npcSpawn = npc.getSpawn();
		if (npcSpawn == null || npcSpawn.getRespawnMinDelay() == TimeSpan.Zero)
		{
			htmlContent.Replace("%respawn%", "None");
		}
		else
		{
			TimeSpan minRespawnDelay = npcSpawn.getRespawnMinDelay();
			TimeSpan maxRespawnDelay = npcSpawn.getRespawnMaxDelay();
			if (npcSpawn.hasRespawnRandom())
			{
				htmlContent.Replace("%respawn%", $"{minRespawnDelay:g}-{maxRespawnDelay:g}");
			}
			else
			{
				htmlContent.Replace("%respawn%", minRespawnDelay.ToString("g"));
			}
		}

		htmlContent.Replace("%atktype%", npc.getAttackType().ToString().toLowerCase().CapitalizeFirstLetter());
		htmlContent.Replace("%atkrange%", npc.getStat().getPhysicalAttackRange().ToString());
		htmlContent.Replace("%patk%", npc.getPAtk().ToString());
		htmlContent.Replace("%pdef%", npc.getPDef().ToString());
		htmlContent.Replace("%matk%", npc.getMAtk().ToString());
		htmlContent.Replace("%mdef%", npc.getMDef().ToString());
		htmlContent.Replace("%atkspd%", npc.getPAtkSpd().ToString());
		htmlContent.Replace("%castspd%", npc.getMAtkSpd().ToString());
		htmlContent.Replace("%critrate%", npc.getStat().getCriticalHit().ToString());
		htmlContent.Replace("%evasion%", npc.getEvasionRate().ToString());
		htmlContent.Replace("%accuracy%", npc.getStat().getAccuracy().ToString());
		htmlContent.Replace("%speed%", ((int)npc.getStat().getMoveSpeed()).ToString());
		htmlContent.Replace("%attributeatktype%", npc.getStat().getAttackElement().ToString());
		htmlContent.Replace("%attributeatkvalue%", npc.getStat().getAttackElementValue(npc.getStat().getAttackElement()).ToString());
		htmlContent.Replace("%attributefire%", npc.getStat().getDefenseElementValue(AttributeType.FIRE).ToString());
		htmlContent.Replace("%attributewater%", npc.getStat().getDefenseElementValue(AttributeType.WATER).ToString());
		htmlContent.Replace("%attributewind%", npc.getStat().getDefenseElementValue(AttributeType.WIND).ToString());
		htmlContent.Replace("%attributeearth%", npc.getStat().getDefenseElementValue(AttributeType.EARTH).ToString());
		htmlContent.Replace("%attributedark%", npc.getStat().getDefenseElementValue(AttributeType.DARK).ToString());
		htmlContent.Replace("%attributeholy%", npc.getStat().getDefenseElementValue(AttributeType.HOLY).ToString());
		htmlContent.Replace("%dropListButtons%", getDropListButtons(npc));

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 0, htmlContent);
		player.sendPacket(html);
	}

	private void sendNpcSkillView(Player player, Npc npc)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/NpcView/Skills.htm", player);

		StringBuilder sb = new();
		npc.getSkills().Values.ForEach(s =>
		{
			sb.Append("<table width=277 height=32 cellspacing=0 background=\"L2UI_CT1.Windows.Windows_DF_TooltipBG\">");
			sb.Append("<tr><td width=32>");
			sb.Append("<img src=\"");
			sb.Append(s.Icon);
			sb.Append("\" width=32 height=32>");
			sb.Append("</td><td width=110>");
			sb.Append(s.Name);
			sb.Append("</td>");
			sb.Append("<td width=45 align=center>");
			sb.Append(s.Id);
			sb.Append("</td>");
			sb.Append("<td width=35 align=center>");
			sb.Append(s.Level);
			sb.Append("</td></tr></table>");
		});

		htmlContent.Replace("%skills%", sb.ToString());
		htmlContent.Replace("%npc_name%", npc.getName());
		htmlContent.Replace("%npcId%", npc.Id.ToString());

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 0, htmlContent);
		player.sendPacket(html);
	}

	private void sendAggroListView(Player player, Npc npc)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/NpcView/AggroList.htm", player);

		StringBuilder sb = new StringBuilder();
		if (npc.isAttackable())
		{
			((Attackable) npc).getAggroList().Values.ForEach(a =>
			{
				sb.Append("<table width=277 height=32 cellspacing=0 background=\"L2UI_CT1.Windows.Windows_DF_TooltipBG\">");
				sb.Append("<tr><td width=110>");
				sb.Append(a.getAttacker() != null ? a.getAttacker().getName() : "NULL");
				sb.Append("</td>");
				sb.Append("<td width=60 align=center>");
				sb.Append(a.getHate());
				sb.Append("</td>");
				sb.Append("<td width=60 align=center>");
				sb.Append(a.getDamage());
				sb.Append("</td></tr></table>");
			});
		}

		htmlContent.Replace("%aggrolist%", sb.ToString());
		htmlContent.Replace("%npc_name%", npc.getName());
		htmlContent.Replace("%npcId%", npc.Id.ToString());
		htmlContent.Replace("%objid%", npc.ObjectId.ToString());

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 0, htmlContent);
		player.sendPacket(html);
	}

	private static string getDropListButtons(Npc npc)
	{
		StringBuilder sb = new StringBuilder();
		List<DropGroupHolder>? dropListGroups = npc.getTemplate().getDropGroups();
		List<DropHolder>? dropListDeath = npc.getTemplate().getDropList();
		List<DropHolder>? dropListSpoil = npc.getTemplate().getSpoilList();
		if (dropListGroups != null || dropListDeath != null || dropListSpoil != null)
		{
			sb.Append("<table width=275 cellpadding=0 cellspacing=0><tr>");
			if (dropListGroups != null || dropListDeath != null)
			{
				sb.Append("<td align=center><button value=\"Show Drop\" width=100 height=25 action=\"bypass NpcViewMod dropList DROP " + npc.ObjectId + "\" back=\"L2UI_CT1.Button_DF_Calculator_Down\" fore=\"L2UI_CT1.Button_DF_Calculator\"></td>");
			}

			if (dropListSpoil != null)
			{
				sb.Append("<td align=center><button value=\"Show Spoil\" width=100 height=25 action=\"bypass NpcViewMod dropList SPOIL " + npc.ObjectId + "\" back=\"L2UI_CT1.Button_DF_Calculator_Down\" fore=\"L2UI_CT1.Button_DF_Calculator\"></td>");
			}

			sb.Append("</tr></table>");
		}
		return sb.ToString();
	}

	private void sendNpcDropList(Player player, Npc npc, DropType dropType, int pageValue)
	{
		List<DropHolder>? dropList = null;
		if (dropType == DropType.SPOIL)
        {
            List<DropHolder>? spoilList = npc.getTemplate().getSpoilList();
			dropList = [];
            if (spoilList != null)
                dropList.AddRange(spoilList);
		}
		else
		{
			List<DropHolder>? drops = npc.getTemplate().getDropList();
			if (drops != null)
			{
				dropList = new(drops);
			}
			List<DropGroupHolder>? dropGroups = npc.getTemplate().getDropGroups();
			if (dropGroups != null)
			{
				if (dropList == null)
				{
					dropList = [];
				}
				foreach (DropGroupHolder dropGroup in dropGroups)
				{
					double chance = dropGroup.getChance() / 100;
					foreach (DropHolder dropHolder in dropGroup.getDropList())
					{
						dropList.Add(new DropHolder(dropHolder.getDropType(), dropHolder.getItemId(),
							dropHolder.getMin(), dropHolder.getMax(), dropHolder.getChance() * chance));
					}
				}
			}
		}
		if (dropList == null)
		{
			return;
		}

		dropList.Sort((d1, d2) => d1.getItemId().CompareTo(d2.getItemId()));

		int pages = dropList.Count / DROP_LIST_ITEMS_PER_PAGE;
		if (DROP_LIST_ITEMS_PER_PAGE * pages < dropList.Count)
		{
			pages++;
		}

		StringBuilder pagesSb = new StringBuilder();
		if (pages > 1)
		{
			pagesSb.Append("<table><tr>");
			for (int i = 0; i < pages; i++)
			{
				pagesSb.Append("<td align=center><button value=\"" + (i + 1) + "\" width=20 height=20 action=\"bypass NpcViewMod dropList " + dropType + " " + npc.ObjectId + " " + i + "\" back=\"L2UI_CT1.Button_DF_Calculator_Down\" fore=\"L2UI_CT1.Button_DF_Calculator\"></td>");
			}
			pagesSb.Append("</tr></table>");
		}

		int page = pageValue;
		if (page >= pages)
		{
			page = pages - 1;
		}

		int start = page > 0 ? page * DROP_LIST_ITEMS_PER_PAGE : 0;
		int end = page * DROP_LIST_ITEMS_PER_PAGE + DROP_LIST_ITEMS_PER_PAGE;
		if (end > dropList.Count)
		{
			end = dropList.Count;
		}

		int leftHeight = 0;
		int rightHeight = 0;
		double dropAmountAdenaEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_ADENA, 1);
		double dropAmountEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_AMOUNT, 1);
		double dropRateEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_RATE, 1);
		double spoilRateEffectBonus = player.getStat().getMul(Stat.BONUS_SPOIL_RATE, 1);
		StringBuilder leftSb = new StringBuilder();
		StringBuilder rightSb = new StringBuilder();
		string limitReachedMsg = "";
		for (int i = start; i < end; i++)
		{
			StringBuilder sb = new StringBuilder();
			int height = 64;
			DropHolder dropItem = dropList[i];
			ItemTemplate? item = ItemData.getInstance().getTemplate(dropItem.getItemId());
            if (item is null)
            {
                _logger.Error("Item template not found for item ID: " + dropItem.getItemId());
                continue;
            }

			// real time server rate calculations
			double rateChance = 1;
			double rateAmount = 1;
			if (dropType == DropType.SPOIL)
			{
				rateChance = Config.Rates.RATE_SPOIL_DROP_CHANCE_MULTIPLIER;
				rateAmount = Config.Rates.RATE_SPOIL_DROP_AMOUNT_MULTIPLIER;

				// also check premium rates if available
				if (Config.PremiumSystem.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
				{
					rateChance *= Config.PremiumSystem.PREMIUM_RATE_SPOIL_CHANCE;
					rateAmount *= Config.PremiumSystem.PREMIUM_RATE_SPOIL_AMOUNT;
				}

				// bonus spoil rate effect
				rateChance *= spoilRateEffectBonus;
			}
			else
			{
				if (Config.Rates.RATE_DROP_CHANCE_BY_ID.TryGetValue(dropItem.getItemId(), out float value))
				{
					rateChance *= value;
				}
				else if (item.hasExImmediateEffect())
				{
					rateChance *= Config.Rates.RATE_HERB_DROP_CHANCE_MULTIPLIER;
				}
				else if (npc.isRaid())
				{
					rateChance *= Config.Rates.RATE_RAID_DROP_CHANCE_MULTIPLIER;
				}
				else
				{
					rateChance *= Config.Rates.RATE_DEATH_DROP_CHANCE_MULTIPLIER;
				}

				if (Config.Rates.RATE_DROP_AMOUNT_BY_ID.TryGetValue(dropItem.getItemId(), out value))
				{
					rateAmount *= value;
				}
				else if (item.hasExImmediateEffect())
				{
					rateAmount *= Config.Rates.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
				}
				else if (npc.isRaid())
				{
					rateAmount *= Config.Rates.RATE_RAID_DROP_AMOUNT_MULTIPLIER;
				}
				else
				{
					rateAmount *= Config.Rates.RATE_DEATH_DROP_AMOUNT_MULTIPLIER;
				}

				// also check premium rates if available
				if (Config.PremiumSystem.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
				{
					if (Config.PremiumSystem.PREMIUM_RATE_DROP_CHANCE_BY_ID.TryGetValue(dropItem.getItemId(), out double value2))
					{
						rateChance *= value2;
					}
					else if (item.hasExImmediateEffect())
					{
						// TODO: Premium herb chance? :)
					}
					else if (npc.isRaid())
					{
						// TODO: Premium raid chance? :)
					}
					else
					{
						rateChance *= Config.PremiumSystem.PREMIUM_RATE_DROP_CHANCE;
					}

					if (Config.PremiumSystem.PREMIUM_RATE_DROP_AMOUNT_BY_ID.TryGetValue(dropItem.getItemId(), out value2))
					{
						rateAmount *= value2;
					}
					else if (item.hasExImmediateEffect())
					{
						// TODO: Premium herb amount? :)
					}
					else if (npc.isRaid())
					{
						// TODO: Premium raid amount? :)
					}
					else
					{
						rateAmount *= Config.PremiumSystem.PREMIUM_RATE_DROP_AMOUNT;
					}
				}

				// bonus drop amount effect
				rateAmount *= dropAmountEffectBonus;
				if (item.Id == Inventory.ADENA_ID)
				{
					rateAmount *= dropAmountAdenaEffectBonus;
				}
				// bonus drop rate effect
				rateChance *= dropRateEffectBonus;
				if (item.Id == Inventory.LCOIN_ID)
				{
					rateChance *= player.getStat().getMul(Stat.BONUS_DROP_RATE_LCOIN, 1);
				}
			}

			sb.Append("<table width=332 cellpadding=2 cellspacing=0 background=\"L2UI_CT1.Windows.Windows_DF_TooltipBG\">");
			sb.Append("<tr><td width=32 valign=top>");
			sb.Append("<button width=\"32\" height=\"32\" back=\"" +
			          (item.getIcon() == null ? "icon.etc_question_mark_i00" : item.getIcon()) + "\" fore=\"" +
			          (item.getIcon() == null ? "icon.etc_question_mark_i00" : item.getIcon()) + "\" itemtooltip=\"" +
			          dropItem.getItemId() + "\">");

			sb.Append("</td><td fixwidth=300 align=center><font name=\"hs9\" color=\"CD9000\">");
			sb.Append(item.getName());
			sb.Append("</font></td></tr><tr><td width=32></td><td width=300><table width=295 cellpadding=0 cellspacing=0>");
			sb.Append("<tr><td width=48 align=right valign=top><font color=\"LEVEL\">Amount:</font></td>");
			sb.Append("<td width=247 align=center>");

			long min = (long)(dropItem.getMin() * rateAmount);
			long max = (long)(dropItem.getMax() * rateAmount);
			if (min == max)
			{
				sb.Append(min.ToString("N0"));
			}
			else
			{
				sb.Append(min.ToString("N0"));
				sb.Append(" - ");
				sb.Append(max.ToString("N0"));
			}

			sb.Append("</td></tr><tr><td width=48 align=right valign=top><font color=\"LEVEL\">Chance:</font></td>");
			sb.Append("<td width=247 align=center>");
			sb.Append(Math.Min(dropItem.getChance() * rateChance, 100).ToString("N4"));
			sb.Append("%</td></tr></table></td></tr><tr><td width=32></td><td width=300>&nbsp;</td></tr></table>");
			if (sb.Length + rightSb.Length + leftSb.Length < 16000) // limit of 32766?
			{
				if (leftHeight >= rightHeight + height)
				{
					rightSb.Append(sb);
					rightHeight += height;
				}
				else
				{
					leftSb.Append(sb);
					leftHeight += height;
				}
			}
			else
			{
				limitReachedMsg = "<br><center>Too many drops! Could not display them all!</center>";
			}
		}

		StringBuilder bodySb = new StringBuilder();
		bodySb.Append("<table><tr>");
		bodySb.Append("<td>");
		bodySb.Append(leftSb.ToString());
		bodySb.Append("</td><td>");
		bodySb.Append(rightSb.ToString());
		bodySb.Append("</td>");
		bodySb.Append("</tr></table>");

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/NpcView/DropList.htm", player);
		htmlContent.Replace("%name%", npc.getName());
		htmlContent.Replace("%dropListButtons%", getDropListButtons(npc));
		htmlContent.Replace("%pages%", pagesSb.ToString());
		htmlContent.Replace("%items%", bodySb + limitReachedMsg);
		Util.sendCBHtml(player, htmlContent.BuildHtml(HtmlActionScope.COMM_BOARD_HTML));
	}
}