using L2Dn.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets.CastleWar;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

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
	private TimeSpan _siegeLength = TimeSpan.FromHours(2); // Changeable in siege.config
	private int _bloodAllianceReward = 0; // Number of Blood Alliance items reward for successful castle defending

	protected SiegeManager()
	{
		load();
	}

	public void addSiegeSkills(Player character)
	{
		foreach (Skill sk in SkillData.getInstance()
			         .getSiegeSkills(character.isNoble(), character.getClan()?.getCastleId() > 0))
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
			int clanId = clan.getId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			register = ctx.SiegeClans.Any(r => r.ClanId == clanId && r.CastleId == castleid);
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
			         .getSiegeSkills(character.isNoble(), character.getClan()?.getCastleId() > 0))
		{
			character.removeSkill(sk);
		}
	}

	private void load()
	{
		ConfigurationParser parser = new ConfigurationParser();
		parser.LoadConfig(Config.SIEGE_CONFIG_FILE);

		// Siege setting
		_siegeCycle = parser.getInt("SiegeCycle", 2);
		_attackerMaxClans = parser.getInt("AttackerMaxClans", 500);
		_attackerRespawnDelay = parser.getInt("AttackerRespawn", 0);
		_defenderMaxClans = parser.getInt("DefenderMaxClans", 500);
		_flagMaxCount = parser.getInt("MaxFlags", 1);
		_siegeClanMinLevel = parser.getInt("SiegeClanMinLevel", 5);
		_siegeLength = TimeSpan.FromMinutes(parser.getInt("SiegeLength", 120));
		_bloodAllianceReward = parser.getInt("BloodAllianceReward", 1);

		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			List<TowerSpawn> controlTowers = new();
			for (int i = 1; i < 0xFF; i++)
			{
				string settingsKeyName = castle.getName() + "ControlTower" + i;
				if (!parser.containsKey(settingsKeyName))
				{
					break;
				}

				StringTokenizer st = new StringTokenizer(parser.getString(settingsKeyName, ""), ",");
				try
				{
					int x = int.Parse(st.nextToken());
					int y = int.Parse(st.nextToken());
					int z = int.Parse(st.nextToken());
					int npcId = int.Parse(st.nextToken());

					controlTowers.Add(new TowerSpawn(npcId, new Location3D(x, y, z)));
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Error while loading control tower(s) for " + castle.getName() +
					            " castle: " + e);
				}
			}

			List<TowerSpawn> flameTowers = new();
			for (int i = 1; i < 0xFF; i++)
			{
				string settingsKeyName = castle.getName() + "FlameTower" + i;
				if (!parser.containsKey(settingsKeyName))
				{
					break;
				}

				StringTokenizer st = new StringTokenizer(parser.getString(settingsKeyName, ""), ",");
				try
				{
					int x = int.Parse(st.nextToken());
					int y = int.Parse(st.nextToken());
					int z = int.Parse(st.nextToken());
					int npcId = int.Parse(st.nextToken());
					List<int> zoneList = new();

					while (st.hasMoreTokens())
					{
						zoneList.Add(int.Parse(st.nextToken()));
					}

					flameTowers.Add(new TowerSpawn(npcId, new Location3D(x, y, z), zoneList));
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Error while loading flame tower(s) for " + castle.getName() +
					            " castle: " + e);
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

	public List<TowerSpawn>? getControlTowers(int castleId)
	{
		return _controlTowers.get(castleId);
	}

	public List<TowerSpawn>? getFlameTowers(int castleId)
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

	public Siege? getSiege(WorldObject activeObject)
	{
		return getSiege(activeObject.Location.Location3D);
	}

	public Siege? getSiege(Location3D location)
	{
		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			if (castle.getSiege().checkIfInZone(location))
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

	public TimeSpan getSiegeLength()
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
			sieges.Add(castle.getSiege());
		}

		return sieges;
	}

	public Siege? getSiege(int castleId)
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (DbCastleTrapUpgrade record in ctx.CastleTrapUpgrades.Where(c => c.CastleId == castleId))
			{
				_flameTowers[castleId][record.TowerIndex].setUpgradeLevel(record.Level);
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
			int diff = (int)(castle.getSiegeDate() - DateTime.UtcNow).TotalMilliseconds;
			if ((diff > 0 && diff < 86400000) || castle.getSiege().isInProgress())
			{
				player.sendPacket(new MercenaryCastleWarCastleSiegeHudInfoPacket(castle.getResidenceId()));
			}
		}
	}

	public void sendSiegeInfo(Player player, int castleId)
	{
		player.sendPacket(new MercenaryCastleWarCastleSiegeHudInfoPacket(castleId));
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