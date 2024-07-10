using System.Text;
using L2Dn.Extensions;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * @author yksdtc
 */
public class DropSearchBoard: IParseBoardHandler
{
	private const String NAVIGATION_PATH = "html/CommunityBoard/Custom/navigation.html";
	private static readonly String[] COMMAND =
	{
		"_bbs_search_item",
		"_bbs_search_drop",
		"_bbs_npc_trace"
	};
	
	private class CBDropHolder
	{
        public readonly int itemId;
        public readonly int npcId;
        public readonly int npcLevel;
        public readonly long min;
        public readonly long max;
        public readonly double chance;
        public readonly bool isSpoil;
        public readonly bool isRaid;
		
		public CBDropHolder(NpcTemplate npcTemplate, DropHolder dropHolder)
		{
			isSpoil = dropHolder.getDropType() == DropType.SPOIL;
			itemId = dropHolder.getItemId();
			npcId = npcTemplate.getId();
			npcLevel = npcTemplate.getLevel();
			min = dropHolder.getMin();
			max = dropHolder.getMax();
			chance = dropHolder.getChance();
			isRaid = npcTemplate.getType().equals("RaidBoss") || npcTemplate.getType().equals("GrandBoss");
		}
		
		/**
		 * only for debug
		 */
		public override String ToString()
		{
			return "DropHolder [itemId=" + itemId + ", npcId=" + npcId + ", npcLevel=" + npcLevel + ", min=" + min + ", max=" + max + ", chance=" + chance + ", isSpoil=" + isSpoil + "]";
		}
	}
	
	private readonly Map<int, List<CBDropHolder>> DROP_INDEX_CACHE = new();
	
	// nonsupport items
	private static readonly Set<int> BLOCK_ID = [Inventory.ADENA_ID];
	
	public DropSearchBoard()
	{
		buildDropIndex();
	}
	
	private void buildDropIndex()
	{
		NpcData.getInstance().getTemplates(npc => npc.getDropGroups() != null).ForEach(npcTemplate =>
		{
			foreach (DropGroupHolder dropGroup in npcTemplate.getDropGroups())
			{
				double chance = dropGroup.getChance() / 100;
				foreach (DropHolder dropHolder in dropGroup.getDropList())
                {
                    addToDropList(npcTemplate,
                        new DropHolder(dropHolder.getDropType(), dropHolder.getItemId(), dropHolder.getMin(),
                            dropHolder.getMax(), dropHolder.getChance() * chance));
                }
			}
		});
		NpcData.getInstance().getTemplates(npc => npc.getDropList() != null).ForEach(npcTemplate =>
		{
			foreach (DropHolder dropHolder in npcTemplate.getDropList())
			{
				addToDropList(npcTemplate, dropHolder);
			}
		});
		NpcData.getInstance().getTemplates(npc => npc.getSpoilList() != null).ForEach(npcTemplate =>
		{
			foreach (DropHolder dropHolder in npcTemplate.getSpoilList())
			{
				addToDropList(npcTemplate, dropHolder);
			}
		});

        DROP_INDEX_CACHE.Values.ForEach(l
            => l.Sort((d1, d2) => d1.npcLevel.CompareTo(d2.npcLevel)));
    }
	
	private void addToDropList(NpcTemplate npcTemplate, DropHolder dropHolder)
	{
		if (BLOCK_ID.Contains(dropHolder.getItemId()))
		{
			return;
		}
		
		List<CBDropHolder> dropList = DROP_INDEX_CACHE.get(dropHolder.getItemId());
		if (dropList == null)
		{
			dropList = new();
			DROP_INDEX_CACHE.put(dropHolder.getItemId(), dropList);
		}
		
		dropList.add(new CBDropHolder(npcTemplate, dropHolder));
	}
	
	public bool parseCommunityBoardCommand(String command, Player player)
    {
        String? navigation = HtmCache.getInstance().getHtm(NAVIGATION_PATH, player.getLang());
		String[] @params = command.Split(" ");
        String? html = HtmCache.getInstance()
            .getHtm("html/CommunityBoard/Custom/dropsearch/main.html", player.getLang());
        
		switch (@params[0])
		{
			case "_bbs_search_item":
			{
				String itemName = buildItemName(@params);
				String result = buildItemSearchResult(itemName);
				html = html.Replace("%searchResult%", result);
				break;
			}
			case "_bbs_search_drop":
			{
				int itemId = int.Parse(@params[1]);
				int page = int.Parse(@params[2]);
				List<CBDropHolder> list = DROP_INDEX_CACHE.get(itemId);
				int pages = list.Count / 14;
				if (pages == 0)
				{
					pages++;
				}
				
				int start = (page - 1) * 14;
				int end = Math.Min(list.Count - 1, start + 14);
				StringBuilder builder = new StringBuilder();
				double dropAmountAdenaEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_ADENA, 1);
				double dropAmountEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_AMOUNT, 1);
				double dropRateEffectBonus = player.getStat().getMul(Stat.BONUS_DROP_RATE, 1);
				double spoilRateEffectBonus = player.getStat().getMul(Stat.BONUS_SPOIL_RATE, 1);
				for (int index = start; index <= end; index++)
				{
					CBDropHolder cbDropHolder = list.get(index);
					
					// real time server rate calculations
					double rateChance = 1;
					double rateAmount = 1;
					if (cbDropHolder.isSpoil)
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
						ItemTemplate? item = ItemData.getInstance().getTemplate(cbDropHolder.itemId);
						if (Config.RATE_DROP_CHANCE_BY_ID.TryGetValue(cbDropHolder.itemId, out float value))
						{
							rateChance *= value;
						}
						else if (item.hasExImmediateEffect())
						{
							rateChance *= Config.RATE_HERB_DROP_CHANCE_MULTIPLIER;
						}
						else if (cbDropHolder.isRaid)
						{
							rateChance *= Config.RATE_RAID_DROP_CHANCE_MULTIPLIER;
						}
						else
						{
							rateChance *= Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER;
						}
						
						if (Config.RATE_DROP_AMOUNT_BY_ID.TryGetValue(cbDropHolder.itemId, out float value1))
						{
							rateAmount *= value1;
						}
						else if (item.hasExImmediateEffect())
						{
							rateAmount *= Config.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
						}
						else if (cbDropHolder.isRaid)
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
							if (Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.TryGetValue(cbDropHolder.itemId, out double value2))
							{
								rateChance *= value2;
							}
							else if (item.hasExImmediateEffect())
							{
								// TODO: Premium herb chance? :)
							}
							else if (cbDropHolder.isRaid)
							{
								// TODO: Premium raid chance? :)
							}
							else
							{
								rateChance *= Config.PREMIUM_RATE_DROP_CHANCE;
							}
							
							if (Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.TryGetValue(cbDropHolder.itemId, out double value3))
							{
								rateAmount *= value3;
							}
							else if (item.hasExImmediateEffect())
							{
								// TODO: Premium herb amount? :)
							}
							else if (cbDropHolder.isRaid)
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
					
					builder.Append("<tr>");
					builder.Append("<td width=30>").Append(cbDropHolder.npcLevel).Append("</td>");
					builder.Append("<td width=170>").Append("<a action=\"bypass _bbs_npc_trace " + cbDropHolder.npcId + "\">").Append("&@").Append(cbDropHolder.npcId).Append(";").Append("</a>").Append("</td>");
					builder.Append("<td width=80 align=CENTER>").Append(cbDropHolder.min * rateAmount).Append("-").Append(cbDropHolder.max * rateAmount).Append("</td>");
					builder.Append("<td width=50 align=CENTER>").Append((cbDropHolder.chance * rateChance).ToString("P4")).Append("%").Append("</td>");
					builder.Append("<td width=50 align=CENTER>").Append(cbDropHolder.isSpoil ? "Spoil" : "Drop").Append("</td>");
					builder.Append("</tr>");
				}
				
				html = html.Replace("%searchResult%", builder.ToString());
				builder.Clear();
				
				builder.Append("<tr>");
				for (page = 1; page <= pages; page++)
				{
					builder.Append("<td>").Append("<a action=\"bypass -h _bbs_search_drop " + itemId + " " + page + " $order $level\">").Append(page).Append("</a>").Append("</td>");
				}
				builder.Append("</tr>");
				html = html.Replace("%pages%", builder.ToString());
				break;
			}
			case "_bbs_npc_trace":
			{
				int npcId = int.Parse(@params[1]);
				List<NpcSpawnTemplate> spawnList = SpawnData.getInstance().getNpcSpawns(npc => npc.getId() == npcId);
				if (spawnList.isEmpty())
				{
					player.sendMessage("Cannot find any spawn. Maybe dropped by a boss or instance monster.");
				}
				else
				{
					NpcSpawnTemplate spawn = spawnList.get(Rnd.get(spawnList.Count));
					Location? spawnLocation = spawn.getSpawnLocation();
					if (spawnLocation != null)
						player.getRadar().addMarker(spawnLocation.Value.X, spawnLocation.Value.Y, spawnLocation.Value.Z);
				}
				break;
			}
		}
		
		if (html != null)
		{
			html = html.Replace("%navigation%", navigation);
			CommunityBoardHandler.separateAndSend(html, player);
		}
		
		return false;
	}
	
	/**
	 * @param itemName
	 * @return
	 */
	private String buildItemSearchResult(String itemName)
	{
		int limit = 0;
		List<ItemTemplate> items = new();
		foreach (ItemTemplate item in ItemData.getInstance().getAllItems())
		{
			if (item == null)
			{
				continue;
			}
			
			if (!DROP_INDEX_CACHE.ContainsKey(item.getId()))
			{
				continue;
			}
			
			if (item.getName().toLowerCase().contains(itemName.toLowerCase()))
			{
				items.add(item);
				limit++;
			}
			
			if (limit == 14)
			{
				break;
			}
		}
		
		if (items.isEmpty())
		{
			return "<tr><td width=100 align=CENTER>No Match</td></tr>";
		}
		
		int line = 0;
		
		StringBuilder builder = new StringBuilder(items.Count * 28);
		int i = 0;
		foreach (ItemTemplate item in items)
		{
			i++;
			if (i == 1)
			{
				line++;
				builder.Append("<tr>");
			}
			
			String icon = item.getIcon();
			if (icon == null)
			{
				icon = "icon.etc_question_mark_i00";
			}
			
			builder.Append("<td>");
			builder.Append("<button value=\".\" action=\"bypass _bbs_search_drop " + item.getId() + " 1 $order $level\" width=32 height=32 back=\"" + icon + "\" fore=\"" + icon + "\">");
			builder.Append("</td>");
			builder.Append("<td width=200>");
			builder.Append("&#").Append(item.getId()).Append(";");
			builder.Append("</td>");
			
			if (i == 2)
			{
				builder.Append("</tr>");
				i = 0;
			}
		}
		
		if ((i % 2) == 1)
		{
			builder.Append("</tr>");
		}
		
		if (line < 7)
		{
			for (i = 0; i < (7 - line); i++)
			{
				builder.Append("<tr><td height=36></td></tr>");
			}
		}
		
		return builder.ToString();
	}
	
	/**
	 * @param params
	 * @return
	 */
	private String buildItemName(String[] @params)
    {
        return string.Join(" ", @params);
	}
	
	public String[] getCommunityBoardCommands()
	{
		return COMMAND;
	}
}
