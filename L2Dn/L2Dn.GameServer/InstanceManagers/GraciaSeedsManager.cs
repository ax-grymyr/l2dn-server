using L2Dn.GameServer.Model.Quests;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

public class GraciaSeedsManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(GraciaSeedsManager));
	
	public static readonly String ENERGY_SEEDS = "EnergySeeds";
	
	private static readonly byte SOITYPE = 2;
	private static readonly byte SOATYPE = 3;
	
	// Seed of Destruction
	private static readonly byte SODTYPE = 1;
	private int _SoDTiatKilled = 0;
	private int _SoDState = 1;
	private readonly Calendar _SoDLastStateChangeDate;
	
	protected GraciaSeedsManager()
	{
		_SoDLastStateChangeDate = Calendar.getInstance();
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
				GlobalVariablesManager.getInstance().set("SoDLSCDate", _SoDLastStateChangeDate.getTimeInMillis());
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
			_SoDLastStateChangeDate.setTimeInMillis(GlobalVariablesManager.getInstance().getLong("SoDLSCDate"));
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
				long timePast = System.currentTimeMillis() - _SoDLastStateChangeDate.getTimeInMillis();
				if (timePast >= Config.SOD_STAGE_2_LENGTH)
				{
					// change to Attack state because Defend statet is not implemented
					setSoDState(1, true);
				}
				else
				{
					ThreadPool.schedule(new UpdateSoDStateTask(), Config.SOD_STAGE_2_LENGTH - timePast);
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
			}
		}
	}
	
	public void updateSodState()
	{
		Quest quest = QuestManager.getInstance().getQuest(ENERGY_SEEDS);
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
			Quest esQuest = QuestManager.getInstance().getQuest(ENERGY_SEEDS);
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
		_SoDLastStateChangeDate.setTimeInMillis(System.currentTimeMillis());
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
	
	public long getSoDTimeForNextStateChange()
	{
		switch (_SoDState)
		{
			case 1:
			{
				return -1;
			}
			case 2:
			{
				return ((_SoDLastStateChangeDate.getTimeInMillis() + Config.SOD_STAGE_2_LENGTH) - System.currentTimeMillis());
			}
			case 3:
			{
				// not implemented yet
				return -1;
			}
			default:
			{
				// this should not happen!
				return -1;
			}
		}
	}
	
	public Calendar getSoDLastStateChangeDate()
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