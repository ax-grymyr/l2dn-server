using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius, Gigi
 */
public class AutoPotionTaskManager: Runnable
{
	private static readonly Set<Player> PLAYERS = new();
	private static bool _working = false;
	
	protected AutoPotionTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(this, 0, 1000);
	}
	
	public void run()
	{
		if (_working)
		{
			return;
		}
		_working = true;
		
		if (!PLAYERS.isEmpty())
		{
			foreach (Player player in PLAYERS)
			{
				if ((player == null) || player.isAlikeDead() || (player.getOnlineStatus() != CharacterOnlineStatus.Online) || (!Config.AUTO_POTIONS_IN_OLYMPIAD && player.isInOlympiadMode()))
				{
					remove(player);
					continue; // player
				}
				
				bool success = false;
				if (Config.AUTO_HP_ENABLED)
				{
					bool restoreHP = ((player.getStatus().getCurrentHp() / player.getMaxHp()) * 100) < Config.AUTO_HP_PERCENTAGE;
					foreach (int itemId in Config.AUTO_HP_ITEM_IDS)
					{
						Item hpPotion = player.getInventory().getItemByItemId(itemId);
						if ((hpPotion != null) && (hpPotion.getCount() > 0))
						{
							success = true;
							if (restoreHP)
							{
								ItemHandler.getInstance().getHandler(hpPotion.getEtcItem()).useItem(player, hpPotion, false);
								player.sendMessage("Auto potion: Restored HP.");
								break;
							}
						}
					}
				}
				if (Config.AUTO_CP_ENABLED)
				{
					bool restoreCP = ((player.getStatus().getCurrentCp() / player.getMaxCp()) * 100) < Config.AUTO_CP_PERCENTAGE;
					foreach (int itemId in Config.AUTO_CP_ITEM_IDS)
					{
						Item cpPotion = player.getInventory().getItemByItemId(itemId);
						if ((cpPotion != null) && (cpPotion.getCount() > 0))
						{
							success = true;
							if (restoreCP)
							{
								ItemHandler.getInstance().getHandler(cpPotion.getEtcItem()).useItem(player, cpPotion, false);
								player.sendMessage("Auto potion: Restored CP.");
								break;
							}
						}
					}
				}
				if (Config.AUTO_MP_ENABLED)
				{
					bool restoreMP = ((player.getStatus().getCurrentMp() / player.getMaxMp()) * 100) < Config.AUTO_MP_PERCENTAGE;
					foreach (int itemId in Config.AUTO_MP_ITEM_IDS)
					{
						Item mpPotion = player.getInventory().getItemByItemId(itemId);
						if ((mpPotion != null) && (mpPotion.getCount() > 0))
						{
							success = true;
							if (restoreMP)
							{
								ItemHandler.getInstance().getHandler(mpPotion.getEtcItem()).useItem(player, mpPotion, false);
								player.sendMessage("Auto potion: Restored MP.");
								break;
							}
						}
					}
				}
				
				if (!success)
				{
					player.sendMessage("Auto potion: You are out of potions!");
				}
			}
		}
		
		_working = false;
	}
	
	public void add(Player player)
	{
		if (!PLAYERS.Contains(player))
		{
			PLAYERS.add(player);
		}
	}
	
	public void remove(Player player)
	{
		PLAYERS.remove(player);
	}
	
	public static AutoPotionTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static AutoPotionTaskManager INSTANCE = new AutoPotionTaskManager();
	}
}