using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using Microsoft.EntityFrameworkCore;
using NLog;
using Pet = L2Dn.GameServer.Model.Actor.Instances.Pet;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

public class Evolve
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Evolve));

	public static bool doEvolve(Player player, Npc npc, int itemIdtake, int itemIdgive, int petminLevel)
	{
		if (itemIdtake == 0 || itemIdgive == 0 || petminLevel == 0)
		{
			return false;
		}

		Summon? pet = player.getPet();
		if (pet == null)
		{
			return false;
		}

		Pet currentPet = (Pet) pet;
		if (currentPet.isAlikeDead())
		{
			Util.handleIllegalPlayerAction(player, player + " tried to use death pet exploit!", Config.DEFAULT_PUNISH);
			return false;
		}

		Item? item = null;
		long petexp = currentPet.getStat().getExp();
		string oldname = currentPet.getName();
		Location3D oldLocation = currentPet.Location.Location3D;
		PetData? oldData = PetDataTable.getInstance().getPetDataByItemId(itemIdtake);
		if (oldData == null)
		{
			return false;
		}

		int oldnpcID = oldData.getNpcId();
		if (currentPet.getStat().getLevel() < petminLevel || currentPet.getId() != oldnpcID)
		{
			return false;
		}

		PetData? petData = PetDataTable.getInstance().getPetDataByItemId(itemIdgive);
		if (petData == null)
		{
			return false;
		}

		int npcID = petData.getNpcId();
		if (npcID == 0)
		{
			return false;
		}

		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(npcID);
		currentPet.unSummon(player);

		// deleting old pet item
		currentPet.destroyControlItem(player, true);
		item = player.getInventory().addItem("Evolve", itemIdgive, 1, player, npc);

		// Summoning new pet
		Pet petSummon = Pet.spawnPet(npcTemplate, player, item);
		if (petSummon == null)
		{
			return false;
		}

		// Fix for non-linear baby pet exp
		long _minimumexp = petSummon.getStat().getExpForLevel(petminLevel);
		if (petexp < _minimumexp)
		{
			petexp = _minimumexp;
		}

		petSummon.getStat().addExp(petexp);
		petSummon.setCurrentHp(petSummon.getMaxHp());
		petSummon.setCurrentMp(petSummon.getMaxMp());
		petSummon.setCurrentFed(petSummon.getMaxFed());
		petSummon.setTitle(player.getName());
		petSummon.setName(oldname);
		petSummon.setRunning();
		petSummon.storeMe();

		player.setPet(petSummon);

		player.sendPacket(new MagicSkillUsePacket(npc, 2046, 1, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(600000)));
		player.sendPacket(SystemMessageId.SUMMONING_YOUR_PET);
		petSummon.spawnMe(oldLocation);
		petSummon.startFeed();
		item.setEnchantLevel(petSummon.getLevel());

		ThreadPool.schedule(new EvolveFinalizer(player, petSummon), 900);
		if (petSummon.getCurrentFed() <= 0)
		{
			ThreadPool.schedule(new EvolveFeedWait(player, petSummon), 60000);
		}
		else
		{
			petSummon.startFeed();
		}

		return true;
	}

	public static bool doRestore(Player player, Npc npc, int itemIdtake, int itemIdgive, int petminLevel)
	{
		if (itemIdtake == 0 || itemIdgive == 0 || petminLevel == 0)
		{
			return false;
		}

		Item? item = player.getInventory().getItemByItemId(itemIdtake);
		if (item == null)
		{
			return false;
		}

		int oldpetlvl = item.getEnchantLevel();
		if (oldpetlvl < petminLevel)
		{
			oldpetlvl = petminLevel;
		}

		PetData? oldData = PetDataTable.getInstance().getPetDataByItemId(itemIdtake);
		if (oldData == null)
		{
			return false;
		}

		PetData? petData = PetDataTable.getInstance().getPetDataByItemId(itemIdgive);
		if (petData == null)
		{
			return false;
		}

		int npcId = petData.getNpcId();
		if (npcId == 0)
		{
			return false;
		}

		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(npcId);

		// deleting old pet item
		Item? removedItem = player.getInventory().destroyItem("PetRestore", item, player, npc);
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
		sm.Params.addItemName(removedItem);
		player.sendPacket(sm);

		// Give new pet item
		Item addedItem = player.getInventory().addItem("PetRestore", itemIdgive, 1, player, npc);

		// Summoning new pet
		Pet? petSummon = Pet.spawnPet(npcTemplate, player, addedItem);
		if (petSummon == null)
		{
			return false;
		}

		long _maxexp = petSummon.getStat().getExpForLevel(oldpetlvl);
		petSummon.getStat().addExp(_maxexp);
		petSummon.setCurrentHp(petSummon.getMaxHp());
		petSummon.setCurrentMp(petSummon.getMaxMp());
		petSummon.setCurrentFed(petSummon.getMaxFed());
		petSummon.setTitle(player.getName());
		petSummon.setRunning();
		petSummon.storeMe();

		player.setPet(petSummon);

		player.sendPacket(new MagicSkillUsePacket(npc, 2046, 1, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(600000)));
		player.sendPacket(SystemMessageId.SUMMONING_YOUR_PET);
		petSummon.spawnMe(player.Location.Location3D);
		petSummon.startFeed();
		addedItem.setEnchantLevel(petSummon.getLevel());

		// Inventory update
		InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(removedItem, ItemChangeType.REMOVED));
		player.sendInventoryUpdate(iu);

		player.broadcastUserInfo();

		ThreadPool.schedule(new EvolveFinalizer(player, petSummon), 900);
		if (petSummon.getCurrentFed() <= 0)
		{
			ThreadPool.schedule(new EvolveFeedWait(player, petSummon), 60000);
		}
		else
		{
			petSummon.startFeed();
		}

		// pet control item no longer exists, delete the pet from the db
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int itemObjectId = removedItem.ObjectId;
			ctx.Pets.Where(p => p.ItemObjectId == itemObjectId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}

		return true;
	}

	private class EvolveFeedWait: Runnable
	{
		private readonly Player _player;
		private readonly Pet _petSummon;

		public EvolveFeedWait(Player player, Pet petSummon)
		{
			_player = player;
			_petSummon = petSummon;
		}

		public void run()
		{
			try
			{
				if (_petSummon.getCurrentFed() <= 0)
				{
					_petSummon.unSummon(_player);
				}
				else
				{
					_petSummon.startFeed();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}
		}
	}

	private class EvolveFinalizer: Runnable
	{
		private readonly Player _player;
		private readonly Pet _petSummon;

		public EvolveFinalizer(Player player, Pet petSummon)
		{
			_player = player;
			_petSummon = petSummon;
		}

		public void run()
		{
			try
			{
				_player.sendPacket(new MagicSkillLaunchedPacket(_player, 2046, 1));
				_petSummon.setFollowStatus(true);
				_petSummon.setShowSummonAnimation(false);
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}
		}
	}
}