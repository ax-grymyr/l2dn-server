using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Quests;

public class QuestTimer
{
	protected readonly string _name;
	protected readonly Quest _quest;
	protected readonly Npc? _npc;
	protected readonly Player _player;
	protected readonly bool _isRepeating;
	protected ScheduledFuture? _scheduler;

	public QuestTimer(Quest quest, string name, TimeSpan time, Npc? npc, Player player, bool repeating)
	{
		_quest = quest;
		_name = name;
		_npc = npc;
		_player = player;
		_isRepeating = repeating;

		if (repeating)
		{
			_scheduler = ThreadPool.scheduleAtFixedRate(new ScheduleTimerTask(this), time, time); // Prepare auto end task
		}
		else
		{
			_scheduler = ThreadPool.schedule(new ScheduleTimerTask(this), time); // Prepare auto end task
		}

		if (npc != null)
		{
			npc.addQuestTimer(this);
		}

		if (player != null)
		{
			player.addQuestTimer(this);
		}
	}

	public void cancel()
	{
		cancelTask();

		if (_npc != null)
		{
			_npc.removeQuestTimer(this);
		}

		if (_player != null)
		{
			_player.removeQuestTimer(this);
		}
	}

	public void cancelTask()
	{
		if (_scheduler != null && !_scheduler.isDone() && !_scheduler.isCancelled())
		{
			_scheduler.cancel(false);
			_scheduler = null;
		}
		_quest.removeQuestTimer(this);
	}

	/**
	 * public method to compare if this timer matches with the key attributes passed.
	 * @param quest : Quest instance to which the timer is attached
	 * @param name : Name of the timer
	 * @param npc : Npc instance attached to the desired timer (null if no npc attached)
	 * @param player : Player instance attached to the desired timer (null if no player attached)
	 * @return bool
	 */
	public bool equals(Quest quest, string name, Npc? npc, Player player)
	{
		if (quest == null || quest != _quest)
		{
			return false;
		}

		if (name == null || !name.equals(_name))
		{
			return false;
		}

		return npc == _npc && player == _player;
	}

	public bool isActive()
	{
		return _scheduler != null && !_scheduler.isCancelled() && !_scheduler.isDone();
	}

	public bool isRepeating()
	{
		return _isRepeating;
	}

	public Quest getQuest()
	{
		return _quest;
	}

	public Npc? getNpc()
	{
		return _npc;
	}

	public Player getPlayer()
	{
		return _player;
	}

	public override string ToString()
	{
		return _name;
	}

	public class ScheduleTimerTask: Runnable
	{
		private readonly QuestTimer _questTimer;

		public ScheduleTimerTask(QuestTimer questTimer)
		{
			_questTimer = questTimer;
		}

		public void run()
		{
			if (_questTimer._scheduler == null)
			{
				return;
			}

			if (!_questTimer._isRepeating)
			{
				_questTimer.cancel();
			}

			_questTimer._quest.notifyEvent(_questTimer._name, _questTimer._npc, _questTimer._player);
		}
	}
}