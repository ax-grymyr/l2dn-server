using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

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
		if (Config.SAVE_DROPPED_ITEM_INTERVAL > 0)
		{
			ThreadPool.scheduleAtFixedRate(this, Config.SAVE_DROPPED_ITEM_INTERVAL, Config.SAVE_DROPPED_ITEM_INTERVAL);
		}
		load();
	}
	
	private void load()
	{
		// If SaveDroppedItem is false, may want to delete all items previously stored to avoid add old items on reactivate
		if (!Config.SAVE_DROPPED_ITEM && Config.CLEAR_DROPPED_ITEM_TABLE)
		{
			emptyTable();
		}
		
		if (!Config.SAVE_DROPPED_ITEM)
		{
			return;
		}
		
		// if DestroyPlayerDroppedItem was previously false, items currently protected will be added to ItemsAutoDestroy
		if (Config.DESTROY_DROPPED_PLAYER_ITEM)
		{
			String str = null;
			if (!Config.DESTROY_EQUIPABLE_PLAYER_ITEM)
			{
				// Recycle misc. items only
				str = "UPDATE itemsonground SET drop_time = ? WHERE drop_time = -1 AND equipable = 0";
			}
			else if (Config.DESTROY_EQUIPABLE_PLAYER_ITEM)
			{
				// Recycle all items including equip-able
				str = "UPDATE itemsonground SET drop_time = ? WHERE drop_time = -1";
			}
			
			try 
			{
				using GameServerDbContext ctx = new();
				PreparedStatement ps = con.prepareStatement(str);
				ps.setLong(1, System.currentTimeMillis());
				ps.execute();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Error while updating table ItemsOnGround " + e.getMessage(), e);
			}
		}
		
		// Add items to world
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement(
					"SELECT object_id,item_id,count,enchant_level,x,y,z,drop_time,equipable FROM itemsonground");
			int count = 0;
			try
			{
				ResultSet rs = ps.executeQuery();
				Item item;
				while (rs.next())
				{
					item = new Item(rs.getInt(1), rs.getInt(2));
					World.getInstance().addObject(item);
					// this check and..
					if (item.isStackable() && (rs.getInt(3) > 1))
					{
						item.setCount(rs.getInt(3));
					}
					// this, are really necessary?
					if (rs.getInt(4) > 0)
					{
						item.setEnchantLevel(rs.getInt(4));
					}
					item.setXYZ(rs.getInt(5), rs.getInt(6), rs.getInt(7));
					item.setWorldRegion(World.getInstance().getRegion(item));
					item.getWorldRegion().addVisibleObject(item);
					long dropTime = rs.getLong(8);
					item.setDropTime(dropTime);
					item.setProtected(dropTime == -1);
					item.setSpawned(true);
					World.getInstance().addVisibleObject(item, item.getWorldRegion());
					_items.add(item);
					count++;
					// add to ItemsAutoDestroy only items not protected
					if (!Config.LIST_PROTECTED_ITEMS.contains(item.getId()) && (dropTime > -1) && (((Config.AUTODESTROY_ITEM_AFTER > 0) && !item.getTemplate().hasExImmediateEffect()) || ((Config.HERB_AUTO_DESTROY_TIME > 0) && item.getTemplate().hasExImmediateEffect())))
					{
						ItemsAutoDestroyTaskManager.getInstance().addItem(item);
					}
				}
			}
			LOGGER.Info(GetType().Name +": Loaded " + count + " items.");
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error while loading ItemsOnGround " + e);
		}
		
		if (Config.EMPTY_DROPPED_ITEM_TABLE_AFTER_LOAD)
		{
			emptyTable();
		}
	}
	
	public void save(Item item)
	{
		if (Config.SAVE_DROPPED_ITEM)
		{
			_items.add(item);
		}
	}
	
	public void removeObject(Item item)
	{
		if (Config.SAVE_DROPPED_ITEM)
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
			using GameServerDbContext ctx = new();
			Statement s = con.createStatement();
			s.executeUpdate("DELETE FROM itemsonground");
		}
		catch (Exception e1)
		{
			LOGGER.Error(GetType().Name + ": Error while cleaning table ItemsOnGround " + e1);
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public synchronized void run()
	{
		if (!Config.SAVE_DROPPED_ITEM)
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
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(
				"INSERT INTO itemsonground(object_id,item_id,count,enchant_level,x,y,z,drop_time,equipable) VALUES(?,?,?,?,?,?,?,?,?)");
			foreach (Item item in _items)
			{
				if (item == null)
				{
					continue;
				}
				
				if (CursedWeaponsManager.getInstance().isCursed(item.getId()))
				{
					continue; // Cursed Items not saved to ground, prevent double save
				}
				
				try
				{
					statement.setInt(1, item.getObjectId());
					statement.setInt(2, item.getId());
					statement.setLong(3, item.getCount());
					statement.setInt(4, item.getEnchantLevel());
					statement.setInt(5, item.getX());
					statement.setInt(6, item.getY());
					statement.setInt(7, item.getZ());
					statement.setLong(8, (item.isProtected() ? -1 : item.getDropTime())); // item is protected or AutoDestroyed
					statement.setLong(9, (item.isEquipable() ? 1 : 0)); // set equip-able
					statement.execute();
					statement.clearParameters();
				}
				catch (Exception e)
				{
					LOGGER.Error(GetType().Name + ": Error while inserting into table ItemsOnGround: " + e);
				}
			}
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