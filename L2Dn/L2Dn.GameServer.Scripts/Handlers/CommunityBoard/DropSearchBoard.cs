using System.Text;
using L2Dn.Extensions;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
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

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * @author yksdtc
 */
public class DropSearchBoard: IParseBoardHandler
{
    private const string _navigationPath = "html/CommunityBoard/Custom/navigation.html";
    private static readonly string[] _commands = ["_bbs_search_item", "_bbs_search_drop", "_bbs_npc_trace"];

    private readonly Map<int, List<CbDropHolder>> _dropIndexCache = new();
    private static readonly Set<int> _blockIds = [Inventory.AdenaId]; // unsupported items

    public DropSearchBoard()
    {
        buildDropIndex();
    }

    private void buildDropIndex()
    {
        NpcData.getInstance().getTemplates(npc => npc.getDropGroups() != null).ForEach(npcTemplate =>
        {
            List<DropGroupHolder>? dropGroups = npcTemplate.getDropGroups();
            if (dropGroups != null)
            {
                foreach (DropGroupHolder dropGroup in dropGroups)
                {
                    double chance = dropGroup.getChance() / 100;
                    foreach (DropHolder dropHolder in dropGroup.getDropList())
                    {
                        addToDropList(npcTemplate,
                            new DropHolder(dropHolder.getDropType(), dropHolder.getItemId(), dropHolder.getMin(),
                                dropHolder.getMax(), dropHolder.getChance() * chance));
                    }
                }
            }
        });

        NpcData.getInstance().getTemplates(npc => npc.getDropList() != null).ForEach(npcTemplate =>
        {
            List<DropHolder>? dropList = npcTemplate.getDropList();
            if (dropList != null)
            {
                foreach (DropHolder dropHolder in dropList)
                {
                    addToDropList(npcTemplate, dropHolder);
                }
            }
        });

        NpcData.getInstance().getTemplates(npc => npc.getSpoilList() != null).ForEach(npcTemplate =>
        {
            List<DropHolder>? spoilList = npcTemplate.getSpoilList();
            if (spoilList != null)
            {
                foreach (DropHolder dropHolder in spoilList)
                {
                    addToDropList(npcTemplate, dropHolder);
                }
            }
        });

        _dropIndexCache.Values.ForEach(l
            => l.Sort((d1, d2) => d1.NpcLevel.CompareTo(d2.NpcLevel)));
    }

    private void addToDropList(NpcTemplate npcTemplate, DropHolder dropHolder)
    {
        if (_blockIds.Contains(dropHolder.getItemId()))
        {
            return;
        }

        List<CbDropHolder>? dropList = _dropIndexCache.get(dropHolder.getItemId());
        if (dropList == null)
        {
            dropList = [];
            _dropIndexCache.put(dropHolder.getItemId(), dropList);
        }

        dropList.Add(new CbDropHolder(npcTemplate, dropHolder));
    }

    public bool parseCommunityBoardCommand(string command, Player player)
    {
        string? navigation = HtmCache.getInstance().getHtm(_navigationPath, player.getLang());
        string[] @params = command.Split(" ");
        string? html = HtmCache.getInstance().
            getHtm("html/CommunityBoard/Custom/dropsearch/main.html", player.getLang());

        switch (@params[0])
        {
            case "_bbs_search_item":
            {
                string itemName = BuildItemName(@params);
                string result = buildItemSearchResult(itemName);
                html = html.Replace("%searchResult%", result);
                break;
            }
            case "_bbs_search_drop":
            {
                int itemId = int.Parse(@params[1]);
                int page = int.Parse(@params[2]);
                List<CbDropHolder> list = _dropIndexCache.get(itemId) ?? [];
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
                    CbDropHolder cbDropHolder = list[index];

                    // real time server rate calculations
                    double rateChance = 1;
                    double rateAmount = 1;
                    if (cbDropHolder.IsSpoil)
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
                        ItemTemplate? item = ItemData.getInstance().getTemplate(cbDropHolder.ItemId);
                        if (Config.Rates.RATE_DROP_CHANCE_BY_ID.TryGetValue(cbDropHolder.ItemId, out float value))
                        {
                            rateChance *= value;
                        }
                        else if (item != null && item.hasExImmediateEffect())
                        {
                            rateChance *= Config.Rates.RATE_HERB_DROP_CHANCE_MULTIPLIER;
                        }
                        else if (cbDropHolder.IsRaid)
                        {
                            rateChance *= Config.Rates.RATE_RAID_DROP_CHANCE_MULTIPLIER;
                        }
                        else
                        {
                            rateChance *= Config.Rates.RATE_DEATH_DROP_CHANCE_MULTIPLIER;
                        }

                        if (Config.Rates.RATE_DROP_AMOUNT_BY_ID.TryGetValue(cbDropHolder.ItemId, out float value1))
                        {
                            rateAmount *= value1;
                        }
                        else if (item != null && item.hasExImmediateEffect())
                        {
                            rateAmount *= Config.Rates.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
                        }
                        else if (cbDropHolder.IsRaid)
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
                            if (Config.PremiumSystem.PREMIUM_RATE_DROP_CHANCE_BY_ID.TryGetValue(cbDropHolder.ItemId,
                                    out double value2))
                            {
                                rateChance *= value2;
                            }
                            else if (item != null && item.hasExImmediateEffect())
                            {
                                // TODO: Premium herb chance? :)
                            }
                            else if (cbDropHolder.IsRaid)
                            {
                                // TODO: Premium raid chance? :)
                            }
                            else
                            {
                                rateChance *= Config.PremiumSystem.PREMIUM_RATE_DROP_CHANCE;
                            }

                            if (Config.PremiumSystem.PREMIUM_RATE_DROP_AMOUNT_BY_ID.TryGetValue(cbDropHolder.ItemId,
                                    out double value3))
                            {
                                rateAmount *= value3;
                            }
                            else if (item != null && item.hasExImmediateEffect())
                            {
                                // TODO: Premium herb amount? :)
                            }
                            else if (cbDropHolder.IsRaid)
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
                        if (item != null && item.Id == Inventory.AdenaId)
                        {
                            rateAmount *= dropAmountAdenaEffectBonus;
                        }

                        // bonus drop rate effect
                        rateChance *= dropRateEffectBonus;
                        if (item != null && item.Id == Inventory.LCOIN_ID)
                        {
                            rateChance *= player.getStat().getMul(Stat.BONUS_DROP_RATE_LCOIN, 1);
                        }
                    }

                    builder.Append("<tr>");
                    builder.Append("<td width=30>").Append(cbDropHolder.NpcLevel).Append("</td>");
                    builder.Append("<td width=170>").
                        Append("<a action=\"bypass _bbs_npc_trace " + cbDropHolder.NpcId + "\">").Append("&@").
                        Append(cbDropHolder.NpcId).Append(";").Append("</a>").Append("</td>");

                    builder.Append("<td width=80 align=CENTER>").Append(cbDropHolder.Min * rateAmount).Append("-").
                        Append(cbDropHolder.Max * rateAmount).Append("</td>");

                    builder.Append("<td width=50 align=CENTER>").
                        Append((cbDropHolder.Chance * rateChance).ToString("P4")).Append("%").Append("</td>");

                    builder.Append("<td width=50 align=CENTER>").Append(cbDropHolder.IsSpoil ? "Spoil" : "Drop").
                        Append("</td>");

                    builder.Append("</tr>");
                }

                html = html.Replace("%searchResult%", builder.ToString());
                builder.Clear();

                builder.Append("<tr>");
                for (page = 1; page <= pages; page++)
                {
                    builder.Append("<td>").
                        Append("<a action=\"bypass -h _bbs_search_drop " + itemId + " " + page + " $order $level\">").
                        Append(page).Append("</a>").Append("</td>");
                }

                builder.Append("</tr>");
                html = html.Replace("%pages%", builder.ToString());
                break;
            }
            case "_bbs_npc_trace":
            {
                int npcId = int.Parse(@params[1]);
                List<NpcSpawnTemplate> spawnList = SpawnData.getInstance().getNpcSpawns(npc => npc.getId() == npcId);
                if (spawnList.Count == 0)
                {
                    player.sendMessage("Cannot find any spawn. Maybe dropped by a boss or instance monster.");
                }
                else
                {
                    NpcSpawnTemplate spawn = spawnList.GetRandomElement();
                    Location? spawnLocation = spawn.getSpawnLocation();
                    if (spawnLocation != null)
                        player.getRadar().addMarker(spawnLocation.Value.X, spawnLocation.Value.Y,
                            spawnLocation.Value.Z);
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
    private string buildItemSearchResult(string itemName)
    {
        int limit = 0;
        List<ItemTemplate> items = [];
        foreach (ItemTemplate item in ItemData.getInstance().getAllItems())
        {
            if (item == null)
            {
                continue;
            }

            if (!_dropIndexCache.ContainsKey(item.Id))
            {
                continue;
            }

            if (item.getName().toLowerCase().contains(itemName.toLowerCase()))
            {
                items.Add(item);
                limit++;
            }

            if (limit == 14)
            {
                break;
            }
        }

        if (items.Count == 0)
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

            string icon = item.getIcon();
            if (icon == null)
            {
                icon = "icon.etc_question_mark_i00";
            }

            builder.Append("<td>");
            builder.Append("<button value=\".\" action=\"bypass _bbs_search_drop " + item.Id +
                " 1 $order $level\" width=32 height=32 back=\"" + icon + "\" fore=\"" + icon + "\">");

            builder.Append("</td>");
            builder.Append("<td width=200>");
            builder.Append("&#").Append(item.Id).Append(";");
            builder.Append("</td>");

            if (i == 2)
            {
                builder.Append("</tr>");
                i = 0;
            }
        }

        if (i % 2 == 1)
        {
            builder.Append("</tr>");
        }

        if (line < 7)
        {
            for (i = 0; i < 7 - line; i++)
            {
                builder.Append("<tr><td height=36></td></tr>");
            }
        }

        return builder.ToString();
    }

    private static string BuildItemName(string[] @params) => string.Join(" ", @params);

    public string[] getCommunityBoardCommands()
    {
        return _commands;
    }

    private sealed class CbDropHolder(NpcTemplate npcTemplate, DropHolder dropHolder)
    {
        public readonly int ItemId = dropHolder.getItemId();
        public readonly int NpcId = npcTemplate.Id;
        public readonly int NpcLevel = npcTemplate.getLevel();
        public readonly long Min = dropHolder.getMin();
        public readonly long Max = dropHolder.getMax();
        public readonly double Chance = dropHolder.getChance();
        public readonly bool IsSpoil = dropHolder.getDropType() == DropType.SPOIL;

        public readonly bool IsRaid =
            npcTemplate.getType().equals("RaidBoss") || npcTemplate.getType().equals("GrandBoss");

        public override string ToString() =>
            $"DropHolder [itemId={ItemId}, npcId={NpcId}, npcLevel={NpcLevel}, min={Min}, max={Max}, chance={Chance}, isSpoil={IsSpoil}]";
    }
}