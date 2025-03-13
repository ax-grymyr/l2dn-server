using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.InstanceManagers.Events;

/**
 * @author Mobius
 */
public class EventDropManager
{
	private static readonly Map<LongTimeEvent, List<EventDropHolder>> EVENT_DROPS = new();

	public void addDrops(LongTimeEvent longTimeEvent, List<EventDropHolder> dropList)
	{
		EVENT_DROPS.put(longTimeEvent, dropList);
	}

	public void removeDrops(LongTimeEvent longTimeEvent)
	{
		EVENT_DROPS.remove(longTimeEvent);
	}

	public void doEventDrop(Creature attacker, Attackable attackable)
	{
		if (EVENT_DROPS.Count == 0)
		{
			return;
		}

		// Event items drop only for players.
        Player? player = attacker.getActingPlayer();
		if (attacker == null || !attacker.isPlayable() || attackable.isFakePlayer() || player == null)
		{
			return;
		}

		// Event items drop only within a default 14 level difference.
		if (player.getLevel() - attackable.getLevel() > Config.EVENT_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE)
		{
			return;
		}

		foreach (List<EventDropHolder> eventDrops in EVENT_DROPS.Values)
		{
			foreach (EventDropHolder drop in eventDrops)
			{
				if (!drop.getMonsterIds().isEmpty() && !drop.getMonsterIds().Contains(attackable.getId()))
				{
					continue;
				}

				int monsterLevel = attackable.getLevel();
				if (monsterLevel >= drop.getMinLevel() && monsterLevel <= drop.getMaxLevel() && Rnd.get(100d) < drop.getChance())
				{
					int itemId = drop.getItemId();
					long itemCount = Rnd.get(drop.getMin(), drop.getMax());
					if (Config.Character.AUTO_LOOT_ITEM_IDS.Contains(itemId) || Config.Character.AUTO_LOOT || attackable.isFlying())
					{
						player.doAutoLoot(attackable, itemId, itemCount); // Give the item to the player that has killed the attackable.
					}
					else
					{
						attackable.dropItem(player, itemId, itemCount); // Drop the item on the ground.
					}
				}
			}
		}
	}

	public static EventDropManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly EventDropManager INSTANCE = new EventDropManager();
	}
}