using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

public class SiegeManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SiegeManager));

	private readonly Map<int, List<TowerSpawn>> _controlTowers = new();
	private readonly Map<int, List<TowerSpawn>> _flameTowers = new();

	private int _siegeCycle = 2; // 2 weeks by default
	private int _attackerMaxClans = 500; // Max number of clans
	private int _attackerRespawnDelay = 0; // Time in ms. Changeable in siege.config
	private int _defenderMaxClans = 500; // Max number of clans
	private int _flagMaxCount = 1; // Changeable in siege.config
	private int _siegeClanMinLevel = 5; // Changeable in siege.config
	private int _siegeLength = 120; // Time in minute. Changeable in siege.config
	private int _bloodAllianceReward = 0; // Number of Blood Alliance items reward for successful castle defending

	protected SiegeManager()
	{
		load();
	}

	public void addSiegeSkills(Player character)
	{
		foreach (Skill sk in SkillData.getInstance()
			         .getSiegeSkills(character.isNoble(), character.getClan().getCastleId() > 0))
		{
			character.addSkill(sk, false);
		}
	}

	/**
	 * @param clan The Clan of the player
	 * @param castleid
	 * @return true if the clan is registered or owner of a castle
	 */
	public bool checkIsRegistered(Clan clan, int castleid)
	{
		if (clan == null)
		{
			return false;
		}

		if (clan.getCastleId() > 0)
		{
			return true;
		}

		bool register = false;
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement =
				con.prepareStatement("SELECT clan_id FROM siege_clans where clan_id=? and castle_id=?");
			statement.setInt(1, clan.getId());
			statement.setInt(2, castleid);
			{
				ResultSet rs = statement.executeQuery();
				if (rs.next())
				{
					register = true;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Exception: checkIsRegistered(): " + e);
		}

		return register;
	}

	public void removeSiegeSkills(Player character)
	{
		foreach (Skill sk in SkillData.getInstance()
			         .getSiegeSkills(character.isNoble(), character.getClan().getCastleId() > 0))
		{
			character.removeSkill(sk);
		}
	}

	private void load()
	{
		PropertiesParser siegeSettings = new PropertiesParser(Config.SIEGE_CONFIG_FILE);

		// Siege setting
		_siegeCycle = siegeSettings.getInt("SiegeCycle", 2);
		_attackerMaxClans = siegeSettings.getInt("AttackerMaxClans", 500);
		_attackerRespawnDelay = siegeSettings.getInt("AttackerRespawn", 0);
		_defenderMaxClans = siegeSettings.getInt("DefenderMaxClans", 500);
		_flagMaxCount = siegeSettings.getInt("MaxFlags", 1);
		_siegeClanMinLevel = siegeSettings.getInt("SiegeClanMinLevel", 5);
		_siegeLength = siegeSettings.getInt("SiegeLength", 120);
		_bloodAllianceReward = siegeSettings.getInt("BloodAllianceReward", 1);

		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			List<TowerSpawn> controlTowers = new();
			for (int i = 1; i < 0xFF; i++)
			{
				String settingsKeyName = castle.getName() + "ControlTower" + i;
				if (!siegeSettings.containskey(settingsKeyName))
				{
					break;
				}

				StringTokenizer st = new StringTokenizer(siegeSettings.getString(settingsKeyName, ""), ",");
				try
				{
					int x = int.Parse(st.nextToken());
					int y = int.Parse(st.nextToken());
					int z = int.Parse(st.nextToken());
					int npcId = int.Parse(st.nextToken());

					controlTowers.add(new TowerSpawn(npcId, new Location(x, y, z)));
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Error while loading control tower(s) for " + castle.getName() +
					            " castle.");
				}
			}

			List<TowerSpawn> flameTowers = new();
			for (int i = 1; i < 0xFF; i++)
			{
				String settingsKeyName = castle.getName() + "FlameTower" + i;
				if (!siegeSettings.containskey(settingsKeyName))
				{
					break;
				}

				StringTokenizer st = new StringTokenizer(siegeSettings.getString(settingsKeyName, ""), ",");
				try
				{
					int x = int.Parse(st.nextToken());
					int y = int.Parse(st.nextToken());
					int z = int.Parse(st.nextToken());
					int npcId = int.Parse(st.nextToken());
					List<int> zoneList = new();

					while (st.hasMoreTokens())
					{
						zoneList.add(int.Parse(st.nextToken()));
					}

					flameTowers.add(new TowerSpawn(npcId, new Location(x, y, z), zoneList));
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Error while loading flame tower(s) for " + castle.getName() +
					            " castle.");
				}
			}

			_controlTowers.put(castle.getResidenceId(), controlTowers);
			_flameTowers.put(castle.getResidenceId(), flameTowers);

			if (castle.getOwnerId() != 0)
			{
				loadTrapUpgrade(castle.getResidenceId());
			}
		}
	}

	public List<TowerSpawn> getControlTowers(int castleId)
	{
		return _controlTowers.get(castleId);
	}

	public List<TowerSpawn> getFlameTowers(int castleId)
	{
		return _flameTowers.get(castleId);
	}

	public int getSiegeCycle()
	{
		return _siegeCycle;
	}

	public int getAttackerMaxClans()
	{
		return _attackerMaxClans;
	}

	public int getAttackerRespawnDelay()
	{
		return _attackerRespawnDelay;
	}

	public int getDefenderMaxClans()
	{
		return _defenderMaxClans;
	}

	public int getFlagMaxCount()
	{
		return _flagMaxCount;
	}

	public Siege getSiege(ILocational loc)
	{
		return getSiege(loc.getX(), loc.getY(), loc.getZ());
	}

	public Siege getSiege(WorldObject activeObject)
	{
		return getSiege(activeObject.getX(), activeObject.getY(), activeObject.getZ());
	}

	public Siege getSiege(int x, int y, int z)
	{
		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			if (castle.getSiege().checkIfInZone(x, y, z))
			{
				return castle.getSiege();
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

	public int getBloodAllianceReward()
	{
		return _bloodAllianceReward;
	}

	public ICollection<Siege> getSieges()
	{
		List<Siege> sieges = new();
		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			sieges.add(castle.getSiege());
		}

		return sieges;
	}

	public Siege getSiege(int castleId)
	{
		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			if (castle.getResidenceId() == castleId)
			{
				return castle.getSiege();
			}
		}

		return null;
	}

	private void loadTrapUpgrade(int castleId)
	{
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM castle_trapupgrade WHERE castleId=?");
			ps.setInt(1, castleId);

			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					_flameTowers.get(castleId).get(rs.getInt("towerIndex")).setUpgradeLevel(rs.getInt("level"));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: loadTrapUpgrade(): " + e);
		}
	}

	public void sendSiegeInfo(Player player)
	{
		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			int diff = (int)(castle.getSiegeDate().getTimeInMillis() - System.currentTimeMillis());
			if (((diff > 0) && (diff < 86400000)) || castle.getSiege().isInProgress())
			{
				player.sendPacket(new MercenaryCastleWarCastleSiegeHudInfo(castle.getResidenceId()));
			}
		}
	}

	public void sendSiegeInfo(Player player, int castleId)
	{
		player.sendPacket(new MercenaryCastleWarCastleSiegeHudInfo(castleId));
	}

	public static SiegeManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly SiegeManager INSTANCE = new SiegeManager();
	}
}