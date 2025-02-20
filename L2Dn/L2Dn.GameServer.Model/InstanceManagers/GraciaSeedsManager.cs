using L2Dn.GameServer.InstanceManagers.Tasks;
using L2Dn.GameServer.Model.Quests;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

public class GraciaSeedsManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(GraciaSeedsManager));

	public const string ENERGY_SEEDS = "EnergySeeds";

	private const byte SOITYPE = 2;
	private const byte SOATYPE = 3;

	// Seed of Destruction
	private const byte SODTYPE = 1;
	private int _SoDTiatKilled = 0;
	private int _SoDState = 1;
	private DateTime _SoDLastStateChangeDate;

	protected GraciaSeedsManager()
	{
		_SoDLastStateChangeDate = DateTime.UtcNow;
		loadData();
		handleSodStages();
	}

	public void saveData(byte seedType)
	{
		switch (seedType)
		{
			case SODTYPE:
			{
				// Seed of Destruction
				GlobalVariablesManager.getInstance().set("SoDState", _SoDState);
				GlobalVariablesManager.getInstance().set("SoDTiatKilled", _SoDTiatKilled);
				GlobalVariablesManager.getInstance().set("SoDLSCDate", _SoDLastStateChangeDate.Ticks);
				break;
			}
			case SOITYPE:
			{
				// Seed of Infinity
				break;
			}
			case SOATYPE:
			{
				// Seed of Annihilation
				break;
			}
			default:
			{
				LOGGER.Warn(GetType().Name + ": Unknown SeedType in SaveData: " + seedType);
				break;
			}
		}
	}

	public void loadData()
	{
		// Seed of Destruction variables
		if (GlobalVariablesManager.getInstance().hasVariable("SoDState"))
		{
			_SoDState = GlobalVariablesManager.getInstance().getInt("SoDState");
			_SoDTiatKilled = GlobalVariablesManager.getInstance().getInt("SoDTiatKilled");
			_SoDLastStateChangeDate = new DateTime(GlobalVariablesManager.getInstance().getLong("SoDLSCDate"));
		}
		else
		{
			// save Initial values
			saveData(SODTYPE);
		}
	}

	private void handleSodStages()
	{
		switch (_SoDState)
		{
			case 1:
			{
				// do nothing, players should kill Tiat a few times
				break;
			}
			case 2:
			{
				// Conquest Complete state, if too much time is passed than change to defense state
				TimeSpan timePast = DateTime.UtcNow - _SoDLastStateChangeDate;
				if (timePast >= TimeSpan.FromMilliseconds(Config.SOD_STAGE_2_LENGTH))
				{
					// change to Attack state because Defend statet is not implemented
					setSoDState(1, true);
				}
				else
				{
					ThreadPool.schedule(new UpdateSoDStateTask(), TimeSpan.FromMilliseconds(Config.SOD_STAGE_2_LENGTH) - timePast);
				}
				break;
			}
			case 3:
			{
				// not implemented
				setSoDState(1, true);
				break;
			}
			default:
			{
				LOGGER.Warn(GetType().Name + ": Unknown Seed of Destruction state(" + _SoDState + ")! ");
				break;
			}
		}
	}

	public void updateSodState()
	{
		Quest? quest = QuestManager.getInstance().getQuest(ENERGY_SEEDS);
		if (quest == null)
		{
			LOGGER.Warn(GetType().Name + ": missing EnergySeeds Quest!");
		}
		else
		{
			quest.notifyEvent("StopSoDAi", null, null);
		}
	}

	public void increaseSoDTiatKilled()
	{
		if (_SoDState == 1)
		{
			_SoDTiatKilled++;
			if (_SoDTiatKilled >= Config.SOD_TIAT_KILL_COUNT)
			{
				setSoDState(2, false);
			}
			saveData(SODTYPE);
			Quest? esQuest = QuestManager.getInstance().getQuest(ENERGY_SEEDS);
			if (esQuest == null)
			{
				LOGGER.Warn(GetType().Name + ": missing EnergySeeds Quest!");
			}
			else
			{
				esQuest.notifyEvent("StartSoDAi", null, null);
			}
		}
	}

	public int getSoDTiatKilled()
	{
		return _SoDTiatKilled;
	}

	public void setSoDState(int value, bool doSave)
	{
		LOGGER.Info(GetType().Name +": New Seed of Destruction state => " + value + ".");
		_SoDLastStateChangeDate = DateTime.UtcNow;
		_SoDState = value;
		// reset number of Tiat kills
		if (_SoDState == 1)
		{
			_SoDTiatKilled = 0;
		}

		handleSodStages();

		if (doSave)
		{
			saveData(SODTYPE);
		}
	}

	public TimeSpan? getSoDTimeForNextStateChange()
	{
		switch (_SoDState)
		{
			case 1:
			{
				return null;
			}
			case 2:
			{
				return _SoDLastStateChangeDate + TimeSpan.FromMilliseconds(Config.SOD_STAGE_2_LENGTH) - DateTime.UtcNow;
			}
			case 3:
			{
				// not implemented yet
				return null;
			}
			default:
			{
				// this should not happen!
				return null;
			}
		}
	}

	public DateTime getSoDLastStateChangeDate()
	{
		return _SoDLastStateChangeDate;
	}

	public int getSoDState()
	{
		return _SoDState;
	}

	/**
	 * Gets the single instance of {@code GraciaSeedsManager}.
	 * @return single instance of {@code GraciaSeedsManager}
	 */
	public static GraciaSeedsManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly GraciaSeedsManager INSTANCE = new GraciaSeedsManager();
	}
}