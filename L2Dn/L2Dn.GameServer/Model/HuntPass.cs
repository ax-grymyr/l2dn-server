using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using NLog;

namespace L2Dn.GameServer.Model;

public class HuntPass
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HuntPass));
	
	private const String INSERT_SEASONPASS = "REPLACE INTO huntpass (`account_name`, `current_step`, `points`, `reward_step`, `is_premium`, `premium_reward_step`, `sayha_points_available`, `sayha_points_used`, `unclaimed_reward`) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
	private const String RESTORE_SEASONPASS = "SELECT * FROM huntpass WHERE account_name=?";
	
	private readonly Player _user;
	private int _availableSayhaTime;
	private int _points;
	private bool _isPremium = false;
	private bool _rewardAlert = false;
	
	private int _rewardStep;
	private int _currentStep;
	private int _premiumRewardStep;
	
	private bool _toggleSayha = false;
	private ScheduledFuture<?> _sayhasSustentionTask = null;
	private int _toggleStartTime = 0;
	private int _usedSayhaTime;
	
	private static int _dayEnd = 0;
	
	public HuntPass(Player user)
	{
		_user = user;
		restoreHuntPass();
		huntPassDayEnd();
		store();
	}
	
	public void restoreHuntPass()
	{
		try 
		{ using GameServerDbContext ctx = new();
			using PreparedStatement statement = con.prepareStatement(RESTORE_SEASONPASS);
			statement.setString(1, getAccountName());
			using ResultSet rset = statement.executeQuery();				
				if (rset.next())
				{
					setPoints(rset.getInt("points"));
					setCurrentStep(rset.getInt("current_step"));
					setRewardStep(rset.getInt("reward_step"));
					setPremium(rset.getBoolean("is_premium"));
					setPremiumRewardStep(rset.getInt("premium_reward_step"));
					setAvailableSayhaTime(rset.getInt("sayha_points_available"));
					setUsedSayhaTime(rset.getInt("sayha_points_used"));
					setRewardAlert(rset.getBoolean("unclaimed_reward"));
				}
				rset.close();
				statement.close();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore Season Pass for playerId: " + _user.getAccountName());
		}
	}
	
	public void resetHuntPass()
	{
		setPoints(0);
		setCurrentStep(0);
		setRewardStep(0);
		setPremium(false);
		setPremiumRewardStep(0);
		setAvailableSayhaTime(0);
		setUsedSayhaTime(0);
		setRewardAlert(false);
		store();
	}
	
	public String getAccountName()
	{
		return _user.getAccountName();
	}
	
	public void store()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			using PreparedStatement statement = con.prepareStatement(INSERT_SEASONPASS);
			statement.setString(1, getAccountName());
			statement.setInt(2, getCurrentStep());
			statement.setInt(3, getPoints());
			statement.setInt(4, getRewardStep());
			statement.setBoolean(5, isPremium());
			statement.setInt(6, getPremiumRewardStep());
			statement.setInt(7, getAvailableSayhaTime());
			statement.setInt(8, getUsedSayhaTime());
			statement.setBoolean(9, rewardAlert());
			statement.execute();
			statement.close();
		}
		catch (Exception e)
		{
			LOGGER.Error( "Could not store Season-Pass data for Account " + _user.getAccountName() + ": ", e);
		}
	}
	
	public int getHuntPassDayEnd()
	{
		return _dayEnd;
	}
	
	public void huntPassDayEnd()
	{
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeInMillis(System.currentTimeMillis());
		calendar.set(Calendar.DAY_OF_MONTH, Config.HUNT_PASS_PERIOD);
		calendar.set(Calendar.HOUR_OF_DAY, 6);
		calendar.set(Calendar.MINUTE, 30);
		if (calendar.getTimeInMillis() < System.currentTimeMillis())
		{
			calendar.add(Calendar.MONTH, 1);
		}
		
		_dayEnd = (int) (calendar.getTimeInMillis() / 1000);
	}
	
	public bool toggleSayha()
	{
		return _toggleSayha;
	}
	
	public int getPoints()
	{
		return _points;
	}
	
	public void addPassPoint()
	{
		if (!Config.ENABLE_HUNT_PASS)
		{
			return;
		}
		
		// Add points.
		int points = getPoints() + 1;
		if (_user.isInTimedHuntingZone())
		{
			points++;
		}
		
		// Check current step.
		bool hasNewLevel = false;
		while (points >= Config.HUNT_PASS_POINTS_FOR_STEP)
		{
			points -= Config.HUNT_PASS_POINTS_FOR_STEP;
			setCurrentStep(getCurrentStep() + 1);
			hasNewLevel = true;
		}
		
		// Save the current point count.
		setPoints(points);
		
		// Send info when needed.
		if (hasNewLevel)
		{
			setRewardAlert(true);
			_user.sendPacket(new HuntPassSimpleInfo(_user));
		}
	}
	
	public void setPoints(int points)
	{
		_points = points;
	}
	
	public int getCurrentStep()
	{
		return _currentStep;
	}
	
	public void setCurrentStep(int step)
	{
		_currentStep = Math.Max(0, Math.Min(step, HuntPassData.getInstance().getRewardsCount()));
	}
	
	public int getRewardStep()
	{
		return _rewardStep;
	}
	
	public void setRewardStep(int step)
	{
		if (_isPremium && (_premiumRewardStep <= _rewardStep))
		{
			return;
		}
		
		_rewardStep = Math.Max(0, Math.Min(step, HuntPassData.getInstance().getRewardsCount()));
	}
	
	public bool isPremium()
	{
		return _isPremium;
	}
	
	public void setPremium(bool premium)
	{
		_isPremium = premium;
	}
	
	public int getPremiumRewardStep()
	{
		return _premiumRewardStep;
	}
	
	public void setPremiumRewardStep(int step)
	{
		_premiumRewardStep = Math.Max(0, Math.Min(step, HuntPassData.getInstance().getPremiumRewardsCount()));
	}
	
	public bool rewardAlert()
	{
		return _rewardAlert;
	}
	
	public void setRewardAlert(bool enable)
	{
		_rewardAlert = enable;
	}
	
	public int getAvailableSayhaTime()
	{
		return _availableSayhaTime;
	}
	
	public void setAvailableSayhaTime(int time)
	{
		_availableSayhaTime = time;
	}
	
	public void addSayhaTime(int time)
	{
		// microsec to sec to database. 1 hour 3600 sec
		_availableSayhaTime += time * 60;
	}
	
	public int getUsedSayhaTime()
	{
		return _usedSayhaTime;
	}
	
	private void onSayhaEndTime()
	{
		setSayhasSustention(false);
	}
	
	public void setUsedSayhaTime(int time)
	{
		_usedSayhaTime = time;
	}
	
	public void addSayhasSustentionTimeUsed(int time)
	{
		_usedSayhaTime += time;
	}
	
	public int getToggleStartTime()
	{
		return _toggleStartTime;
	}
	
	public void setSayhasSustention(bool active)
	{
		_toggleSayha = active;
		if (active)
		{
			_toggleStartTime = (int) (System.currentTimeMillis() / 1000);
			if (_sayhasSustentionTask != null)
			{
				_sayhasSustentionTask.cancel(true);
				_sayhasSustentionTask = null;
			}
			_user.sendPacket(SystemMessageId.SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_IS_ACTIVATED_AVAILABLE_SAYHA_S_GRACE_SUSTENTION_TIME_IS_BEING_CONSUMED);
			_sayhasSustentionTask = ThreadPool.schedule(this::onSayhaEndTime, Math.Max(0, getAvailableSayhaTime() - getUsedSayhaTime()) * 1000L);
		}
		else
		{
			if (_sayhasSustentionTask != null)
			{
				addSayhasSustentionTimeUsed((int) ((System.currentTimeMillis() / 1000) - _toggleStartTime));
				_toggleStartTime = 0;
				_sayhasSustentionTask.cancel(true);
				_sayhasSustentionTask = null;
				_user.sendPacket(SystemMessageId.SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_HAS_BEEN_DEACTIVATED_THE_SUSTENTION_TIME_YOU_HAVE_DOES_NOT_DECREASE);
			}
		}
	}
}
