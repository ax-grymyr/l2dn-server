using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

public class HuntPass
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HuntPass));

	private readonly Player _user;
	private int _availableSayhaTime;
	private int _points;
	private bool _isPremium;
	private bool _rewardAlert;

	private int _rewardStep;
	private int _currentStep;
	private int _premiumRewardStep;

	private bool _toggleSayha;
	private ScheduledFuture? _sayhasSustentionTask;
	private DateTime? _toggleStartTime;
	private int _usedSayhaTime;

	private static DateTime _dayEnd;

	public HuntPass(Player user)
	{
		_user = user;
	}

	public void restoreHuntPass()
	{
		try
		{
			int accountId = _user.getAccountId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (Db.DbHuntPass record in ctx.HuntPasses.Where(r => r.AccountId == accountId))
			{
				setPoints(record.Points);
				setCurrentStep(record.CurrentStep);
				setRewardStep(record.RewardStep);
				setPremium(record.IsPremium);
				setPremiumRewardStep(record.PremiumRewardStep);
				setAvailableSayhaTime(record.SayhaPointsAvailable);
				setUsedSayhaTime(record.SayhaPointsUsed);
				setRewardAlert(record.UnclaimedReward);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore Season Pass for playerId: " + _user.getAccountName() + ": " + e);
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

	public string getAccountName()
	{
		return _user.getAccountName();
	}

	public void store()
	{
		try
		{
			int accountId = _user.getAccountId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			Db.DbHuntPass? huntPass = ctx.HuntPasses.SingleOrDefault(r => r.AccountId == accountId);
			if (huntPass is null)
			{
				huntPass = new Db.DbHuntPass();
				huntPass.AccountId = accountId;
				ctx.HuntPasses.Add(huntPass);
			}

			huntPass.CurrentStep = getCurrentStep();
			huntPass.Points = getPoints();
			huntPass.RewardStep = getRewardStep();
			huntPass.IsPremium = isPremium();
			huntPass.RewardStep = getRewardStep();
			huntPass.PremiumRewardStep = getPremiumRewardStep();
			huntPass.SayhaPointsAvailable = getAvailableSayhaTime();
			huntPass.SayhaPointsUsed = getUsedSayhaTime();
			huntPass.UnclaimedReward = rewardAlert();

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error( "Could not store Season-Pass data for Account " + _user.getAccountName() + ": ", e);
		}
	}

	public DateTime getHuntPassDayEnd()
	{
		return _dayEnd;
	}

	public void huntPassDayEnd()
	{
		DateTime calendar = DateTime.Now;
		calendar = new DateTime(calendar.Year, calendar.Month, Config.HUNT_PASS_PERIOD, 6, 30, 0);
		if (calendar < DateTime.Now)
		{
			calendar = calendar.AddMonths(1);
		}

		_dayEnd = calendar;
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
			_user.sendPacket(new HuntPassSimpleInfoPacket(_user));
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
		if (_isPremium && _premiumRewardStep <= _rewardStep)
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

	public DateTime? getToggleStartTime()
	{
		return _toggleStartTime;
	}

	public void setSayhasSustention(bool active)
	{
		_toggleSayha = active;
		if (active)
		{
			_toggleStartTime = DateTime.UtcNow;
			if (_sayhasSustentionTask != null)
			{
				_sayhasSustentionTask.cancel(true);
				_sayhasSustentionTask = null;
			}
			_user.sendPacket(SystemMessageId.SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_IS_ACTIVATED_AVAILABLE_SAYHA_S_GRACE_SUSTENTION_TIME_IS_BEING_CONSUMED);
			_sayhasSustentionTask = ThreadPool.schedule(onSayhaEndTime, Math.Max(0, getAvailableSayhaTime() - getUsedSayhaTime()) * 1000);
		}
		else
		{
			if (_sayhasSustentionTask != null)
			{
				addSayhasSustentionTimeUsed((int)(DateTime.UtcNow - _toggleStartTime ?? TimeSpan.Zero).TotalSeconds); // TODO: time arithmetic
				_toggleStartTime = null;
				_sayhasSustentionTask.cancel(true);
				_sayhasSustentionTask = null;
				_user.sendPacket(SystemMessageId.SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_HAS_BEEN_DEACTIVATED_THE_SUSTENTION_TIME_YOU_HAVE_DOES_NOT_DECREASE);
			}
		}
	}
}