using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Restoration Random effect implementation.
/// This effect is present in item skills that "extract" new items upon usage.
/// This effect has been unhardcoded in order to work on targets as well.
/// </summary>
public sealed class RestorationRandom: AbstractEffect
{
    private readonly ImmutableArray<ExtractableProductItem> _products;

    public RestorationRandom(StatSet @params)
    {
        List<ExtractableProductItem> products = new(16);
        List<XmlSkillEffectItemListChance>? xmlSkillEffectItemList =
            @params.getObject<List<XmlSkillEffectItemListChance>>("items");

        if (xmlSkillEffectItemList != null)
        {
            foreach (XmlSkillEffectItemListChance xmlSkillEffectItemListChance in xmlSkillEffectItemList)
            {
                List<RestorationItemHolder> items = xmlSkillEffectItemListChance.Items.Select(xmlSkillEffectItem =>
                    new RestorationItemHolder(xmlSkillEffectItem.Id, xmlSkillEffectItem.Count,
                        xmlSkillEffectItem.MinEnchant, xmlSkillEffectItem.MaxEnchant)).ToList();

                products.Add(new ExtractableProductItem(items.ToImmutableArray(),
                    (double)xmlSkillEffectItemListChance.Chance));
            }
        }
        else
        {
            List<StatSet>? sets = @params.getList<StatSet>("items");
            if (sets != null)
            {
                List<RestorationItemHolder> items = new(16);
                foreach (StatSet group in sets)
                {
                    items.Clear();
                    List<StatSet>? itemSets = group.getList<StatSet>(".");
                    if (itemSets != null)
                    {
                        foreach (StatSet item in itemSets)
                        {
                            items.Add(new RestorationItemHolder(item.getInt(".id"), item.getInt(".count"),
                                item.getInt(".minEnchant", 0), item.getInt(".maxEnchant", 0)));
                        }
                    }

                    products.Add(new ExtractableProductItem(items.ToImmutableArray(), group.getFloat(".chance")));
                }
            }
        }

        _products = products.ToImmutableArray();
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        double rndNum = 100 * Rnd.nextDouble();
        double chanceFrom = 0;
        List<RestorationItemHolder> creationList = [];

        // Explanation for future changes:
        // You get one chance for the current skill, then you can fall into
        // one of the "areas" like in a roulette.
        // Example: for an item like Id1,A1,30;Id2,A2,50;Id3,A3,20;
        // #---#-----#--#
        // 0--30----80-100
        // If you get chance equal 45% you fall into the second zone 30-80.
        // Meaning you get the second production list.
        // Calculate extraction
        foreach (ExtractableProductItem expi in _products)
        {
            double chance = expi.Chance;
            if (rndNum >= chanceFrom && rndNum <= chance + chanceFrom)
            {
                creationList.AddRange(expi.Items);
                break;
            }

            chanceFrom += chance;
        }

        if (creationList.Count == 0)
        {
            player.sendPacket(SystemMessageId.FAILED_TO_CHANGE_THE_ITEM);
            return;
        }

        Map<Item, long> extractedItems = new();
        foreach (RestorationItemHolder createdItem in creationList)
        {
            if (createdItem.Id <= 0 || createdItem.Count <= 0)
                continue;

            long itemCount = (long)(createdItem.Count * Config.Rates.RATE_EXTRACTABLE);
            Item? newItem = player.addItem("Extract", createdItem.Id, itemCount, effector, false);
            if (newItem == null)
                continue;

            if (createdItem.MaxEnchant > 0)
                newItem.setEnchantLevel(Rnd.get(createdItem.MinEnchant, createdItem.MaxEnchant));

            if (extractedItems.ContainsKey(newItem))
                extractedItems.put(newItem, extractedItems.get(newItem) + itemCount);
            else
                extractedItems.put(newItem, itemCount);
        }

        if (extractedItems.Count != 0)
        {
            List<ItemInfo> items = [];
            foreach ((Item key, long value) in extractedItems)
            {
                if (key.getTemplate().isStackable())
                    items.Add(new ItemInfo(key, ItemChangeType.MODIFIED));
                else
                {
                    items.AddRange(player.getInventory().getAllItemsByItemId(key.Id).
                        Select(itemInstance => new ItemInfo(itemInstance, ItemChangeType.MODIFIED)));
                }

                SendMessage(player, key, value);
            }

            InventoryUpdatePacket playerIU = new InventoryUpdatePacket(items);
            player.sendPacket(playerIU);
        }
    }

    public override EffectTypes EffectType => EffectTypes.EXTRACT_ITEM;

    private static void SendMessage(Player player, Item item, long count)
    {
        SystemMessagePacket sm;
        if (count > 1)
        {
            sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
            sm.Params.addItemName(item);
            sm.Params.addLong(count);
        }
        else if (item.getEnchantLevel() > 0)
        {
            sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2);
            sm.Params.addInt(item.getEnchantLevel());
            sm.Params.addItemName(item);
        }
        else
        {
            sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1);
            sm.Params.addItemName(item);
        }

        player.sendPacket(sm);
    }

    public override int GetHashCode() => _products.GetSequenceHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._products.GetSequentialComparable());

    public bool EqualsApproximately(RestorationRandom other) // TODO: remove after migrating Skill loader to XML deserialization
    {
        if (_products.Length != other._products.Length)
            return false;

        return _products.Zip(other._products).All(pair =>
            pair.First.Items.SequenceEqual(pair.Second.Items) && pair.First.Chance - 0.001 <= pair.Second.Chance &&
            pair.Second.Chance <= pair.First.Chance + 0.001);
    }

    private readonly record struct ExtractableProductItem(ImmutableArray<RestorationItemHolder> Items, double Chance)
    {
        public override int GetHashCode() => HashCode.Combine(Items.GetSequenceHashCode(), Chance);

        public bool Equals(ExtractableProductItem other) =>
            Items.SequenceEqual(other.Items) && Chance.Equals(other.Chance);
    }

    private readonly record struct RestorationItemHolder(int Id, long Count, int MinEnchant, int MaxEnchant);
}