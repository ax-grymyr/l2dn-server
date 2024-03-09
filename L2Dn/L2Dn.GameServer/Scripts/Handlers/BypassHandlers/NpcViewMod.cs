using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers.BypassHandlers;

/**
 * @author NosBit
 */
public class NpcViewMod: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(NpcViewMod));

	private static readonly string[] COMMANDS =
	{
		"NpcViewMod"
	};
	
	private const int DROP_LIST_ITEMS_PER_PAGE = 10;
	
	public bool useBypass(String command, Player player, Creature bypassOrigin)
	{
		StringTokenizer st = new StringTokenizer(command);
		st.nextToken();
		
		if (!st.hasMoreTokens())
		{
			_logger.Warn("Bypass[NpcViewMod] used without enough parameters.");
			return false;
		}
		
		String actualCommand = st.nextToken();
		switch (actualCommand.toLowerCase())
		{
			case "view":
			{
				WorldObject target;
				if (st.hasMoreElements())
				{
					try
					{
						target = World.getInstance().findObject(int.Parse(st.nextToken()));
					}
					catch (FormatException e)
					{
						return false;
					}
				}
				else
				{
					target = player.getTarget();
				}
				
				Npc npc = target is Npc ? (Npc) target : null;
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
				
				String dropListTypeString = st.nextToken();
				try
				{
					DropType dropListType = Enum.Parse<DropType>(dropListTypeString);
					WorldObject target = World.getInstance().findObject(int.Parse(st.nextToken()));
					Npc npc = target is Npc ? (Npc) target : null;
					if (npc == null)
					{
						return false;
					}
					int page = st.hasMoreElements() ? int.Parse(st.nextToken()) : 0;
					sendNpcDropList(player, npc, dropListType, page);
				}
				catch (FormatException e)
				{
					return false;
				}
				catch (ArgumentException e)
				{
					_logger.Warn("Bypass[NpcViewMod] unknown drop list scope: " + dropListTypeString);
					return false;
				}
				break;
			}
			case "skills":
			{
				WorldObject target;
				if (st.hasMoreElements())
				{
					try
					{
						target = World.getInstance().findObject(int.Parse(st.nextToken()));
					}
					catch (FormatException e)
					{
						return false;
					}
				}
				else
				{
					target = player.getTarget();
				}
				
				Npc npc = target is Npc ? (Npc) target : null;
				if (npc == null)
				{
					return false;
				}
				
				sendNpcSkillView(player, npc);
				break;
			}
			case "aggrolist":
			{
				WorldObject target;
				if (st.hasMoreElements())
				{
					try
					{
						target = World.getInstance().findObject(int.Parse(st.nextToken()));
					}
					catch (FormatException e)
					{
						return false;
					}
				}
				else
				{
					target = player.getTarget();
				}
				
				Npc npc = target is Npc ? (Npc) target : null;
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
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
	
	public static void sendNpcView(Player player, Npc npc)
	{
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/mods/NpcView/Info.htm");
		helper.Replace("%name%", npc.getName());
		helper.Replace("%hpGauge%", HtmlUtil.getHpGauge(250, (long) npc.getCurrentHp(), npc.getMaxHp(), false));
		helper.Replace("%mpGauge%", HtmlUtil.getMpGauge(250, (long) npc.getCurrentMp(), npc.getMaxMp(), false));
		
		Spawn npcSpawn = npc.getSpawn();
		if ((npcSpawn == null) || (npcSpawn.getRespawnMinDelay() == TimeSpan.Zero))
		{
			helper.Replace("%respawn%", "None");
		}
		else
		{
			TimeSpan minRespawnDelay = npcSpawn.getRespawnMinDelay();
			TimeSpan maxRespawnDelay = npcSpawn.getRespawnMaxDelay();
			if (npcSpawn.hasRespawnRandom())
			{
				helper.Replace("%respawn%", $"{minRespawnDelay:g}-{maxRespawnDelay:g}");
			}
			else
			{
				helper.Replace("%respawn%", minRespawnDelay.ToString("g"));
			}
		}
		
		helper.Replace("%atktype%", CommonUtil.capitalizeFirst(npc.getAttackType().ToString().toLowerCase()));
		helper.Replace("%atkrange%", npc.getStat().getPhysicalAttackRange().ToString());
		helper.Replace("%patk%", npc.getPAtk().ToString());
		helper.Replace("%pdef%", npc.getPDef().ToString());
		helper.Replace("%matk%", npc.getMAtk().ToString());
		helper.Replace("%mdef%", npc.getMDef().ToString());
		helper.Replace("%atkspd%", npc.getPAtkSpd().ToString());
		helper.Replace("%castspd%", npc.getMAtkSpd().ToString());
		helper.Replace("%critrate%", npc.getStat().getCriticalHit().ToString());
		helper.Replace("%evasion%", npc.getEvasionRate().ToString());
		helper.Replace("%accuracy%", npc.getStat().getAccuracy().ToString());
		helper.Replace("%speed%", ((int)npc.getStat().getMoveSpeed()).ToString());
		helper.Replace("%attributeatktype%", npc.getStat().getAttackElement().ToString());
		helper.Replace("%attributeatkvalue%", npc.getStat().getAttackElementValue(npc.getStat().getAttackElement()).ToString());
		helper.Replace("%attributefire%", npc.getStat().getDefenseElementValue(AttributeType.FIRE).ToString());
		helper.Replace("%attributewater%", npc.getStat().getDefenseElementValue(AttributeType.WATER).ToString());
		helper.Replace("%attributewind%", npc.getStat().getDefenseElementValue(AttributeType.WIND).ToString());
		helper.Replace("%attributeearth%", npc.getStat().getDefenseElementValue(AttributeType.EARTH).ToString());
		helper.Replace("%attributedark%", npc.getStat().getDefenseElementValue(AttributeType.DARK).ToString());
		helper.Replace("%attributeholy%", npc.getStat().getDefenseElementValue(AttributeType.HOLY).ToString());
		helper.Replace("%dropListButtons%", getDropListButtons(npc));

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(helper);
		player.sendPacket(html);
	}
	
	private void sendNpcSkillView(Player player, Npc npc)
	{
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/mods/NpcView/Skills.htm");
		
		StringBuilder sb = new StringBuilder();
		npc.getSkills().values().forEach(s =>
		{
			sb.Append("<table width=277 height=32 cellspacing=0 background=\"L2UI_CT1.Windows.Windows_DF_TooltipBG\">");
			sb.Append("<tr><td width=32>");
			sb.Append("<img src=\"");
			sb.Append(s.getIcon());
			sb.Append("\" width=32 height=32>");
			sb.Append("</td><td width=110>");
			sb.Append(s.getName());
			sb.Append("</td>");
			sb.Append("<td width=45 align=center>");
			sb.Append(s.getId());
			sb.Append("</td>");
			sb.Append("<td width=35 align=center>");
			sb.Append(s.getLevel());
			sb.Append("</td></tr></table>");
		});
		
		helper.Replace("%skills%", sb.ToString());
		helper.Replace("%npc_name%", npc.getName());
		helper.Replace("%npcId%", npc.getId().ToString());

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(helper);
		player.sendPacket(html);
	}
	
	private void sendAggroListView(Player player, Npc npc)
	{
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/mods/NpcView/AggroList.htm");
		
		StringBuilder sb = new StringBuilder();
		if (npc.isAttackable())
		{
			((Attackable) npc).getAggroList().values().forEach(a =>
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
		
		helper.Replace("%aggrolist%", sb.ToString());
		helper.Replace("%npc_name%", npc.getName());
		helper.Replace("%npcId%", npc.getId().ToString());
		helper.Replace("%objid%", npc.getObjectId().ToString());

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(helper);
		player.sendPacket(html);
	}
	
	private static String getDropListButtons(Npc npc)
	{
		StringBuilder sb = new StringBuilder();
		List<DropGroupHolder> dropListGroups = npc.getTemplate().getDropGroups();
		List<DropHolder> dropListDeath = npc.getTemplate().getDropList();
		List<DropHolder> dropListSpoil = npc.getTemplate().getSpoilList();
		if ((dropListGroups != null) || (dropListDeath != null) || (dropListSpoil != null))
		{
			sb.Append("<table width=275 cellpadding=0 cellspacing=0><tr>");
			if ((dropListGroups != null) || (dropListDeath != null))
			{
				sb.Append("<td align=center><button value=\"Show Drop\" width=100 height=25 action=\"bypass NpcViewMod dropList DROP " + npc.getObjectId() + "\" back=\"L2UI_CT1.Button_DF_Calculator_Down\" fore=\"L2UI_CT1.Button_DF_Calculator\"></td>");
			}
			
			if (dropListSpoil != null)
			{
				sb.Append("<td align=center><button value=\"Show Spoil\" width=100 height=25 action=\"bypass NpcViewMod dropList SPOIL " + npc.getObjectId() + "\" back=\"L2UI_CT1.Button_DF_Calculator_Down\" fore=\"L2UI_CT1.Button_DF_Calculator\"></td>");
			}
			
			sb.Append("</tr></table>");
		}
		return sb.ToString();
	}
	
	private void sendNpcDropList(Player player, Npc npc, DropType dropType, int pageValue)
	{
		List<DropHolder> dropList = null;
		if (dropType == DropType.SPOIL)
		{
			dropList = new(npc.getTemplate().getSpoilList());
		}
		else
		{
			List<DropHolder> drops = npc.getTemplate().getDropList();
			if (drops != null)
			{
				dropList = new(drops);
			}
			List<DropGroupHolder> dropGroups = npc.getTemplate().getDropGroups();
			if (dropGroups != null)
			{
				if (dropList == null)
				{
					dropList = new();
				}
				foreach (DropGroupHolder dropGroup in dropGroups)
				{
					double chance = dropGroup.getChance() / 100;
					foreach (DropHolder dropHolder in dropGroup.getDropList())
					{
						dropList.add(new DropHolder(dropHolder.getDropType(), dropHolder.getItemId(),
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
		
		int pages = dropList.size() / DROP_LIST_ITEMS_PER_PAGE;
		if ((DROP_LIST_ITEMS_PER_PAGE * pages) < dropList.size())
		{
			pages++;
		}
		
		StringBuilder pagesSb = new StringBuilder();
		if (pages > 1)
		{
			pagesSb.Append("<table><tr>");
			for (int i = 0; i < pages; i++)
			{
				pagesSb.Append("<td align=center><button value=\"" + (i + 1) + "\" width=20 height=20 action=\"bypass NpcViewMod dropList " + dropType + " " + npc.getObjectId() + " " + i + "\" back=\"L2UI_CT1.Button_DF_Calculator_Down\" fore=\"L2UI_CT1.Button_DF_Calculator\"></td>");
			}
			pagesSb.Append("</tr></table>");
		}
		
		int page = pageValue;
		if (page >= pages)
		{
			page = pages - 1;
		}
		
		int start = page > 0 ? page * DROP_LIST_ITEMS_PER_PAGE : 0;
		int end = (page * DROP_LIST_ITEMS_PER_PAGE) + DROP_LIST_ITEMS_PER_PAGE;
		if (end > dropList.size())
		{
			end = dropList.size();
		}
		
		int leftHeight = 0;
		int rightHeight = 0;
		double dropAmountAdenaEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_ADENA, 1);
		double dropAmountEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_AMOUNT, 1);
		double dropRateEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_RATE, 1);
		double spoilRateEffectBonus = player.getStat().getMul(Stat.BONUS_SPOIL_RATE, 1);
		StringBuilder leftSb = new StringBuilder();
		StringBuilder rightSb = new StringBuilder();
		String limitReachedMsg = "";
		for (int i = start; i < end; i++)
		{
			StringBuilder sb = new StringBuilder();
			int height = 64;
			DropHolder dropItem = dropList.get(i);
			ItemTemplate item = ItemData.getInstance().getTemplate(dropItem.getItemId());
			
			// real time server rate calculations
			double rateChance = 1;
			double rateAmount = 1;
			if (dropType == DropType.SPOIL)
			{
				rateChance = Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER;
				rateAmount = Config.RATE_SPOIL_DROP_AMOUNT_MULTIPLIER;
				
				// also check premium rates if available
				if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
				{
					rateChance *= Config.PREMIUM_RATE_SPOIL_CHANCE;
					rateAmount *= Config.PREMIUM_RATE_SPOIL_AMOUNT;
				}
				
				// bonus spoil rate effect
				rateChance *= spoilRateEffectBonus;
			}
			else
			{
				if (Config.RATE_DROP_CHANCE_BY_ID.TryGetValue(dropItem.getItemId(), out float value))
				{
					rateChance *= value;
				}
				else if (item.hasExImmediateEffect())
				{
					rateChance *= Config.RATE_HERB_DROP_CHANCE_MULTIPLIER;
				}
				else if (npc.isRaid())
				{
					rateChance *= Config.RATE_RAID_DROP_CHANCE_MULTIPLIER;
				}
				else
				{
					rateChance *= Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER;
				}
				
				if (Config.RATE_DROP_AMOUNT_BY_ID.TryGetValue(dropItem.getItemId(), out value))
				{
					rateAmount *= value;
				}
				else if (item.hasExImmediateEffect())
				{
					rateAmount *= Config.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
				}
				else if (npc.isRaid())
				{
					rateAmount *= Config.RATE_RAID_DROP_AMOUNT_MULTIPLIER;
				}
				else
				{
					rateAmount *= Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER;
				}
				
				// also check premium rates if available
				if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
				{
					if (Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.TryGetValue(dropItem.getItemId(), out double value2))
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
						rateChance *= Config.PREMIUM_RATE_DROP_CHANCE;
					}
					
					if (Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.TryGetValue(dropItem.getItemId(), out value2))
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
						rateAmount *= Config.PREMIUM_RATE_DROP_AMOUNT;
					}
				}
				
				// bonus drop amount effect
				rateAmount *= dropAmountEffectBonus;
				if (item.getId() == Inventory.ADENA_ID)
				{
					rateAmount *= dropAmountAdenaEffectBonus;
				}
				// bonus drop rate effect
				rateChance *= dropRateEffectBonus;
				if (item.getId() == Inventory.LCOIN_ID)
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
			if ((sb.Length + rightSb.Length + leftSb.Length) < 16000) // limit of 32766?
			{
				if (leftHeight >= (rightHeight + height))
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

		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/mods/NpcView/DropList.htm");
		helper.Replace("%name%", npc.getName());
		helper.Replace("%dropListButtons%", getDropListButtons(npc));
		helper.Replace("%pages%", pagesSb.ToString());
		helper.Replace("%items%", bodySb.ToString() + limitReachedMsg);
		Util.sendCBHtml(player, helper.getHtml());
	}
}