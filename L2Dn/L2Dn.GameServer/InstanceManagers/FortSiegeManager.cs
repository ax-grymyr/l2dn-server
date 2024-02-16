using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.InstanceManagers;

public class FortSiegeManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FortSiegeManager));
	
	private int _attackerMaxClans = 500; // Max number of clans
	
	// Fort Siege settings
	private Map<int, List<FortSiegeSpawn>> _commanderSpawnList;
	private Map<int, List<CombatFlag>> _flagList;
	private bool _justToTerritory = true; // Changeable in fortsiege.properties
	private int _flagMaxCount = 1; // Changeable in fortsiege.properties
	private int _siegeClanMinLevel = 4; // Changeable in fortsiege.properties
	private int _siegeLength = 60; // Time in minute. Changeable in fortsiege.properties
	private int _countDownLength = 10; // Time in minute. Changeable in fortsiege.properties
	private int _suspiciousMerchantRespawnDelay = 180; // Time in minute. Changeable in fortsiege.properties
	private readonly Map<int, FortSiege> _sieges = new();
	
	protected FortSiegeManager()
	{
		load();
	}
	
	public void addSiegeSkills(Player character)
	{
		character.addSkill(CommonSkill.SEAL_OF_RULER.getSkill(), false);
		character.addSkill(CommonSkill.BUILD_HEADQUARTERS.getSkill(), false);
	}
	
	public void addCombatFlaglagSkills(Player character)
	{
		Clan clan = character.getClan();
		if ((clan != null))
		{
			if ((clan.getLevel() >= getSiegeClanMinLevel()) && FortManager.getInstance().getFortById(FortManager.ORC_FORTRESS).getSiege().isInProgress())
			{
				character.addSkill(CommonSkill.FLAG_DISPLAY.getSkill(), false);
				character.addSkill(CommonSkill.REMOTE_FLAG_DISPLAY.getSkill(), false);
				character.addSkill(CommonSkill.FLAG_POWER_FAST_RUN.getSkill(), false);
				character.addSkill(CommonSkill.FLAG_EQUIP.getSkill(), false);
				switch (character.getClassId())
				{
					// Warrior
					case CharacterClass.DUELIST:
					case CharacterClass.DREADNOUGHT:
					case CharacterClass.TITAN:
					case CharacterClass.GRAND_KHAVATARI:
					case CharacterClass.FORTUNE_SEEKER:
					case CharacterClass.MAESTRO:
					case CharacterClass.DOOMBRINGER:
					case CharacterClass.SOUL_HOUND:
					case CharacterClass.DEATH_KIGHT_HUMAN:
					case CharacterClass.DEATH_KIGHT_ELF:
					case CharacterClass.DEATH_KIGHT_DARK_ELF:
					{
						character.addSkill(CommonSkill.FLAG_POWER_WARRIOR.getSkill(), false);
						break;
					}
					// Knight
					case CharacterClass.PHOENIX_KNIGHT:
					case CharacterClass.HELL_KNIGHT:
					case CharacterClass.EVA_TEMPLAR:
					case CharacterClass.SHILLIEN_TEMPLAR:
					{
						character.addSkill(CommonSkill.FLAG_POWER_KNIGHT.getSkill(), false);
						break;
					}
					// Rogue
					case CharacterClass.ADVENTURER:
					case CharacterClass.WIND_RIDER:
					case CharacterClass.GHOST_HUNTER:
					{
						character.addSkill(CommonSkill.FLAG_POWER_ROGUE.getSkill(), false);
						break;
					}
					// Archer
					case CharacterClass.SAGITTARIUS:
					case CharacterClass.MOONLIGHT_SENTINEL:
					case CharacterClass.GHOST_SENTINEL:
					case CharacterClass.TRICKSTER:
					{
						character.addSkill(CommonSkill.FLAG_POWER_ARCHER.getSkill(), false);
						break;
					}
					// Mage
					case CharacterClass.ARCHMAGE:
					case CharacterClass.SOULTAKER:
					case CharacterClass.MYSTIC_MUSE:
					case CharacterClass.STORM_SCREAMER:
					{
						character.addSkill(CommonSkill.FLAG_POWER_MAGE.getSkill(), false);
						break;
					}
					// Summoner
					case CharacterClass.ARCANA_LORD:
					case CharacterClass.ELEMENTAL_MASTER:
					case CharacterClass.SPECTRAL_MASTER:
					{
						character.addSkill(CommonSkill.FLAG_POWER_SUMMONER.getSkill(), false);
						break;
					}
					// Healer
					case CharacterClass.CARDINAL:
					case CharacterClass.EVA_SAINT:
					case CharacterClass.SHILLIEN_SAINT:
					{
						character.addSkill(CommonSkill.FLAG_POWER_HEALER.getSkill(), false);
						break;
					}
					// Enchanter
					case CharacterClass.HIEROPHANT:
					{
						character.addSkill(CommonSkill.FLAG_POWER_ENCHANTER.getSkill(), false);
						break;
					}
					// Bard
					case CharacterClass.SWORD_MUSE:
					case CharacterClass.SPECTRAL_DANCER:
					{
						character.addSkill(CommonSkill.FLAG_POWER_BARD.getSkill(), false);
						break;
					}
					// Shaman
					case CharacterClass.DOMINATOR:
					case CharacterClass.DOOMCRYER:
					{
						character.addSkill(CommonSkill.FLAG_POWER_SHAMAN.getSkill(), false);
						break;
					}
				}
			}
		}
	}
	
	public void removeCombatFlagSkills(Player character)
	{
		character.removeSkill(CommonSkill.FLAG_DISPLAY.getSkill());
		character.removeSkill(CommonSkill.REMOTE_FLAG_DISPLAY.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_FAST_RUN.getSkill());
		character.removeSkill(CommonSkill.FLAG_EQUIP.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_WARRIOR.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_KNIGHT.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_ROGUE.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_ARCHER.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_MAGE.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_SUMMONER.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_HEALER.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_ENCHANTER.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_BARD.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_SHAMAN.getSkill());
		character.removeSkill(CommonSkill.FLAG_POWER_ENCHANTER.getSkill());
		character.removeSkill(CommonSkill.FLAG_EQUIP.getSkill());
	}
	
	/**
	 * @param clan The Clan of the player
	 * @param fortid
	 * @return true if the clan is registered or owner of a fort
	 */
	public bool checkIsRegistered(Clan clan, int fortid)
	{
		if (clan == null)
		{
			return false;
		}
		
		bool register = false;
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement("SELECT clan_id FROM fortsiege_clans where clan_id=? and fort_id=?");
			ps.setInt(1, clan.getId());
			ps.setInt(2, fortid);

			{
				ResultSet rs = ps.executeQuery();
				if (rs.next())
				{
					register = true;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: checkIsRegistered(): " + e);
		}
		return register;
	}
	
	public void removeSiegeSkills(Player character)
	{
		character.removeSkill(CommonSkill.SEAL_OF_RULER.getSkill());
		character.removeSkill(CommonSkill.BUILD_HEADQUARTERS.getSkill());
	}
	
	private void load()
	{
		Properties siegeSettings = new Properties();
		File file = new File(Config.FORTSIEGE_CONFIG_FILE);
		try
		{
			InputStream is1  = new FileInputStream(file);
			siegeSettings.load(is1);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while loading Fort Siege Manager settings!" + e);
		}
		
		// Siege setting
		_justToTerritory = Boolean.parseBoolean(siegeSettings.getProperty("JustToTerritory", "true"));
		_attackerMaxClans = int.decode(siegeSettings.getProperty("AttackerMaxClans", "500"));
		_flagMaxCount = int.decode(siegeSettings.getProperty("MaxFlags", "1"));
		_siegeClanMinLevel = int.decode(siegeSettings.getProperty("SiegeClanMinLevel", "4"));
		_siegeLength = int.decode(siegeSettings.getProperty("SiegeLength", "60"));
		_countDownLength = int.decode(siegeSettings.getProperty("CountDownLength", "10"));
		_suspiciousMerchantRespawnDelay = int.decode(siegeSettings.getProperty("SuspiciousMerchantRespawnDelay", "180"));
		
		// Siege spawns settings
		_commanderSpawnList = new();
		_flagList = new();
		foreach (Fort fort in FortManager.getInstance().getForts())
		{
			List<FortSiegeSpawn> commanderSpawns = new();
			List<CombatFlag> flagSpawns = new();
			for (int i = 1; i < 5; i++)
			{
				String _spawnParams = siegeSettings.getProperty(fort.getName().Replace(" ", "") + "Commander" + i, "");
				if (_spawnParams.isEmpty())
				{
					break;
				}
				
				StringTokenizer st = new StringTokenizer(_spawnParams.Trim(), ",");
				
				try
				{
					int x = int.Parse(st.nextToken());
					int y = int.Parse(st.nextToken());
					int z = int.Parse(st.nextToken());
					int heading = int.Parse(st.nextToken());
					int npc_id = int.Parse(st.nextToken());
					commanderSpawns.add(new FortSiegeSpawn(fort.getResidenceId(), x, y, z, heading, npc_id, i));
				}
				catch (Exception e)
				{
					LOGGER.Warn("Error while loading commander(s) for " + fort.getName() + " fort.");
				}
			}
			
			_commanderSpawnList.put(fort.getResidenceId(), commanderSpawns);
			
			for (int i = 1; i < 4; i++)
			{
				String _spawnParams = siegeSettings.getProperty(fort.getName().Replace(" ", "") + "Flag" + i, "");
				if (_spawnParams.isEmpty())
				{
					break;
				}
				StringTokenizer st = new StringTokenizer(_spawnParams.Trim(), ",");
				
				try
				{
					int x = int.Parse(st.nextToken());
					int y = int.Parse(st.nextToken());
					int z = int.Parse(st.nextToken());
					int flag_id = int.Parse(st.nextToken());
					flagSpawns.add(new CombatFlag(fort.getResidenceId(), x, y, z, 0, flag_id));
				}
				catch (Exception e)
				{
					LOGGER.Warn("Error while loading flag(s) for " + fort.getName() + " fort.");
				}
			}
			_flagList.put(fort.getResidenceId(), flagSpawns);
		}
	}
	
	public List<FortSiegeSpawn> getCommanderSpawnList(int fortId)
	{
		return _commanderSpawnList.get(fortId);
	}
	
	public List<CombatFlag> getFlagList(int fortId)
	{
		return _flagList.get(fortId);
	}
	
	public int getAttackerMaxClans()
	{
		return _attackerMaxClans;
	}
	
	public int getFlagMaxCount()
	{
		return _flagMaxCount;
	}
	
	public bool canRegisterJustTerritory()
	{
		return _justToTerritory;
	}
	
	public int getSuspiciousMerchantRespawnDelay()
	{
		return _suspiciousMerchantRespawnDelay;
	}
	
	public FortSiege getSiege(WorldObject activeObject)
	{
		return getSiege(activeObject.getX(), activeObject.getY(), activeObject.getZ());
	}
	
	public FortSiege getSiege(int x, int y, int z)
	{
		foreach (Fort fort in FortManager.getInstance().getForts())
		{
			if (fort.getSiege().checkIfInZone(x, y, z))
			{
				return fort.getSiege();
			}
		}
		return null;
	}
	
	public int getSiegeClanMinLevel()
	{
		return _siegeClanMinLevel;
	}
	
	public int getSiegeLength()
	{
		return _siegeLength;
	}
	
	public int getCountDownLength()
	{
		return _countDownLength;
	}
	
	public ICollection<FortSiege> getSieges()
	{
		return _sieges.values();
	}
	
	public FortSiege getSiege(int fortId)
	{
		return _sieges.get(fortId);
	}
	
	public void addSiege(FortSiege fortSiege)
	{
		_sieges.put(fortSiege.getFort().getResidenceId(), fortSiege);
	}
	
	public bool isCombat(int itemId)
	{
		return itemId == FortManager.ORC_FORTRESS_FLAG;
	}
	
	public bool activateCombatFlag(Player player, Item item)
	{
		if (!checkIfCanPickup(player))
		{
			return false;
		}
		
		if (player.isMounted())
		{
			player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
		}
		else
		{
			player.getInventory().equipItem(item);
			
			InventoryUpdate iu = new InventoryUpdate();
			iu.addItem(item);
			player.sendInventoryUpdate(iu);
			
			player.broadcastUserInfo();
			player.setCombatFlagEquipped(true);
			addCombatFlaglagSkills(player);
			
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_EQUIPPED);
			sm.Params.addItemName(item);
			player.sendPacket(sm);
		}
		
		return true;
	}
	
	public bool checkIfCanPickup(Player player)
	{
		if (player.isCombatFlagEquipped())
		{
			return false;
		}
		
		Fort fort = FortManager.getInstance().getFort(player);
		// if ((fort == null) || (fort.getResidenceId() <= 0) || (fort.getSiege().getAttackerClan(player.getClan()) == null))
		if ((fort == null) || (fort.getResidenceId() <= 0))
		{
			return false;
		}
		
		if (!fort.getSiege().isInProgress())
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_OF_S1_HAS_FINISHED);
			sm.Params.addItemName(FortManager.ORC_FORTRESS_FLAG);
			player.sendPacket(sm);
			return false;
		}
		
		return true;
	}
	
	public void dropCombatFlag(Player player, int fortId)
	{
		Fort fort = FortManager.getInstance().getFortById(fortId);
		if (player != null)
		{
			removeCombatFlagSkills(player);
			long slot = player.getInventory().getSlotFromItem(player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG));
			player.getInventory().unEquipItemInBodySlot(slot);
			Item flag = player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG);
			player.destroyItem("CombatFlag", flag, null, true);
			player.setCombatFlagEquipped(false);
			player.broadcastUserInfo();
			InventoryUpdate iu = new InventoryUpdate();
			player.sendInventoryUpdate(iu);
			SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName(flag.getVariables().getString(FortSiege.GREG_SPAWN_VAR, FortSiege.ORC_FORTRESS_GREG_BOTTOM_RIGHT_SPAWN)).forEach(holder =>
			{
				holder.spawnAll();
				foreach (NpcSpawnTemplate nst in holder.getSpawns())
				{
					foreach (Npc npc in nst.getSpawnedNpcs())
					{
						Spawn spawn = npc.getSpawn();
						if (spawn != null)
						{
							spawn.stopRespawn();
						}
					}
				}
			}));
			
		}
		fort.getSiege().addFlagCount(-1);
	}
	
	public static FortSiegeManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly FortSiegeManager INSTANCE = new FortSiegeManager();
	}
}