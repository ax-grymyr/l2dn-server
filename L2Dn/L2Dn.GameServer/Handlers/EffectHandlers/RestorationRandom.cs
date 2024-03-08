using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Restoration Random effect implementation.<br>
 * This effect is present in item skills that "extract" new items upon usage.<br>
 * This effect has been unhardcoded in order to work on targets as well.
 * @author Zoey76, Mobius
 */
public class RestorationRandom: AbstractEffect
{
	private readonly List<ExtractableProductItem> _products = new();

	public RestorationRandom(StatSet @params)
	{
		foreach (StatSet group in @params.getList<StatSet>("items"))
		{
			List<RestorationItemHolder> items = new();
			foreach (StatSet item in group.getList<StatSet>("."))
			{
				items.add(new RestorationItemHolder(item.getInt(".id"), item.getInt(".count"),
					item.getInt(".minEnchant", 0), item.getInt(".maxEnchant", 0)));
			}

			_products.add(new ExtractableProductItem(items, group.getFloat(".chance")));
		}
	}

	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		double rndNum = 100 * Rnd.nextDouble();
		double chance = 0;
		double chanceFrom = 0;
		List<RestorationItemHolder> creationList = new();
		
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
			chance = expi.getChance();
			if ((rndNum >= chanceFrom) && (rndNum <= (chance + chanceFrom)))
			{
				creationList.AddRange(expi.getItems());
				break;
			}
			chanceFrom += chance;
		}
		
		Player player = effected.getActingPlayer();
		if (creationList.isEmpty())
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CHANGE_THE_ITEM);
			return;
		}
		
		Map<Item, long> extractedItems = new();
		foreach (RestorationItemHolder createdItem in creationList)
		{
			if ((createdItem.getId() <= 0) || (createdItem.getCount() <= 0))
			{
				continue;
			}
			
			long itemCount = (long) (createdItem.getCount() * Config.RATE_EXTRACTABLE);
			Item newItem = player.addItem("Extract", createdItem.getId(), itemCount, effector, false);
			
			if (createdItem.getMaxEnchant() > 0)
			{
				newItem.setEnchantLevel(Rnd.get(createdItem.getMinEnchant(), createdItem.getMaxEnchant()));
			}
			
			if (extractedItems.containsKey(newItem))
			{
				extractedItems.put(newItem, extractedItems.get(newItem) + itemCount);
			}
			else
			{
				extractedItems.put(newItem, itemCount);
			}
		}
		
		if (!extractedItems.isEmpty())
		{
			List<ItemInfo> items = new List<ItemInfo>();
			foreach (var entry in extractedItems)
			{
				if (entry.Key.getTemplate().isStackable())
				{
					items.Add(new ItemInfo(entry.Key, ItemChangeType.MODIFIED));
				}
				else
				{
					foreach (Item itemInstance in player.getInventory().getAllItemsByItemId(entry.Key.getId()))
					{
						items.Add(new ItemInfo(itemInstance, ItemChangeType.MODIFIED));
					}
				}
				
				sendMessage(player, entry.Key, entry.Value);
			}

			InventoryUpdatePacket playerIU = new InventoryUpdatePacket(items);
			player.sendPacket(playerIU);
		}
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.EXTRACT_ITEM;
	}
	
	private void sendMessage(Player player, Item item, long count)
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
}