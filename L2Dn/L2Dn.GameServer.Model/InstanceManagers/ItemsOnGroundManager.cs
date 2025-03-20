using System.Runtime.CompilerServices;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * This class manage all items on ground.
 * @author Enforcer
 */
public class ItemsOnGroundManager: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemsOnGroundManager));

	private readonly Set<Item> _items = new();

	protected ItemsOnGroundManager()
	{
		if (Config.General.SAVE_DROPPED_ITEM_INTERVAL > 0)
		{
			ThreadPool.scheduleAtFixedRate(this, Config.General.SAVE_DROPPED_ITEM_INTERVAL, Config.General.SAVE_DROPPED_ITEM_INTERVAL);
		}
		load();
	}

	private void load()
	{
		// If SaveDroppedItem is false, may want to delete all items previously stored to avoid add old items on reactivate
		if (!Config.General.SAVE_DROPPED_ITEM && Config.General.CLEAR_DROPPED_ITEM_TABLE)
		{
			emptyTable();
		}

		if (!Config.General.SAVE_DROPPED_ITEM)
		{
			return;
		}

		// if DestroyPlayerDroppedItem was previously false, items currently protected will be added to ItemsAutoDestroy
		if (Config.General.DESTROY_DROPPED_PLAYER_ITEM)
		{
			try
			{
				DateTime time = DateTime.UtcNow;

				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				var query = ctx.ItemsOnGround.Where(r => r.DropTime == null);

				if (!Config.General.DESTROY_EQUIPABLE_PLAYER_ITEM)
					query = query.Where(r => !r.Equipable);

				query.ExecuteUpdate(s => s.SetProperty(r => r.DropTime, time));
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Error while updating table ItemsOnGround: " + e);
			}
		}

		// Add items to world
		try
		{
			int count = 0;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (DbItemOnGround itemOnGround in ctx.ItemsOnGround)
			{
				Item item;
				item = new Item(itemOnGround.ObjectId, itemOnGround.ItemId);
				World.getInstance().addObject(item);
				// this check and..
				if (item.isStackable() && itemOnGround.Count > 1)
				{
					item.setCount(itemOnGround.Count);
				}
				// this, are really necessary?
				if (itemOnGround.EnchantLevel > 0)
				{
					item.setEnchantLevel(itemOnGround.EnchantLevel);
				}

				item.setXYZ(itemOnGround.X, itemOnGround.Y, itemOnGround.Z);
                WorldRegion? region = World.getInstance().getRegion(item);
                if (region == null)
                {
                    LOGGER.Error("Region not found for item " + item.Id + " " + item.getName());
                    continue;
                }

				item.setWorldRegion(region);
				region.AddVisibleObject(item);
				DateTime? dropTime = itemOnGround.DropTime;
				item.setDropTime(dropTime ?? DateTime.MinValue); // TODO
				item.setProtected(dropTime is null);
				item.setSpawned(true);
				World.getInstance().addVisibleObject(item, region);
				_items.add(item);
				count++;
				// add to ItemsAutoDestroy only items not protected
				if (!Config.General.LIST_PROTECTED_ITEMS.Contains(item.Id) && dropTime is not null &&
				    ((Config.General.AUTODESTROY_ITEM_AFTER > 0 && !item.getTemplate().hasExImmediateEffect()) ||
				     (Config.General.HERB_AUTO_DESTROY_TIME > 0 && item.getTemplate().hasExImmediateEffect())))
				{
					ItemsAutoDestroyTaskManager.getInstance().addItem(item);
				}
			}

			LOGGER.Info(GetType().Name +": Loaded " + count + " items.");
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error while loading ItemsOnGround " + e);
		}

		if (Config.General.EMPTY_DROPPED_ITEM_TABLE_AFTER_LOAD)
		{
			emptyTable();
		}
	}

	public void save(Item item)
	{
		if (Config.General.SAVE_DROPPED_ITEM)
		{
			_items.add(item);
		}
	}

	public void removeObject(Item item)
	{
		if (Config.General.SAVE_DROPPED_ITEM)
		{
			_items.remove(item);
		}
	}

	public void saveInDb()
	{
		run();
	}

	public void cleanUp()
	{
		_items.clear();
	}

	public void emptyTable()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ItemsOnGround.ExecuteDelete();
		}
		catch (Exception e1)
		{
			LOGGER.Error(GetType().Name + ": Error while cleaning table ItemsOnGround " + e1);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void run()
	{
		if (!Config.General.SAVE_DROPPED_ITEM)
		{
			return;
		}

		emptyTable();

		if (_items.isEmpty())
		{
			return;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			foreach (Item item in _items)
			{
				if (item == null)
				{
					continue;
				}

				if (CursedWeaponsManager.getInstance().isCursed(item.Id))
				{
					continue; // Cursed Items not saved to ground, prevent double save
				}

				ctx.ItemsOnGround.Add(new DbItemOnGround()
				{
					ObjectId = item.ObjectId,
					ItemId = item.Id,
					Count = item.getCount(),
					EnchantLevel = item.getEnchantLevel(),
					X = item.getX(),
					Y = item.getY(),
					Z = item.getZ(),
					DropTime = item.isProtected() ? null : item.getDropTime(),
					Equipable = item.isEquipable()
				});
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": SQL error while storing items on ground: " + e);
		}
	}

	/**
	 * Gets the single instance of {@code ItemsOnGroundManager}.
	 * @return single instance of {@code ItemsOnGroundManager}
	 */
	public static ItemsOnGroundManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ItemsOnGroundManager INSTANCE = new ItemsOnGroundManager();
	}
}