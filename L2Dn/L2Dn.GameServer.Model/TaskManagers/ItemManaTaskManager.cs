using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class ItemManaTaskManager: Runnable
{
    private const int MANA_CONSUMPTION_RATE = 60000;
    private static readonly Map<Item, DateTime> _items = [];
    private static bool _working;

    protected ItemManaTaskManager()
    {
        ThreadPool.scheduleAtFixedRate(this, 1000, 1000);
    }

    public void run()
    {
        if (_working)
        {
            return;
        }

        _working = true;

        if (_items.Count != 0)
        {
            DateTime currentTime = DateTime.UtcNow;
            List<Item> toRemove = [];
            foreach (KeyValuePair<Item, DateTime> entry in _items)
            {
                if (currentTime > entry.Value)
                {
                    Item item = entry.Key;
                    toRemove.Add(item);

                    Player? player = item.getActingPlayer();
                    if (player == null || player.isInOfflineMode())
                    {
                        continue;
                    }

                    item.decreaseMana(item.isEquipped());
                }
            }

            foreach (Item item in toRemove)
            {
                _items.remove(item);
            }
        }

        _working = false;
    }

    public void add(Item item)
    {
        if (!_items.ContainsKey(item))
        {
            _items.put(item, DateTime.UtcNow.AddMilliseconds(MANA_CONSUMPTION_RATE));
        }
    }

    public static ItemManaTaskManager getInstance()
    {
        return SingletonHolder.Instance;
    }

    private static class SingletonHolder
    {
        public static readonly ItemManaTaskManager Instance = new();
    }
}