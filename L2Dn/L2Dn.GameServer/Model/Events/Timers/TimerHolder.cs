using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Events.Timers;

public abstract class TimerHolder: Runnable
{
	private readonly StatSet _params;
	private readonly Npc _npc;
	private readonly Player _player;
	private readonly bool _isRepeating;

	protected TimerHolder(StatSet @params, Npc npc, Player player, bool isRepeating)
	{
		_params = @params;
		_npc = npc;
		_player = player;
		_isRepeating = isRepeating;
	}

	/**
	 * @return the parameters of this timer
	 */
	public StatSet getParams()
	{
		return _params;
	}

	/**
	 * @return the npc of this timer
	 */
	public Npc getNpc()
	{
		return _npc;
	}

	/**
	 * @return the player of this timer
	 */
	public Player getPlayer()
	{
		return _player;
	}

	/**
	 * @return {@code true} if the timer will repeat itself, {@code false} otherwise
	 */
	public bool isRepeating()
	{
		return _isRepeating;
	}

	/**
	 * Cancels this timer.
	 */
	public abstract void cancelTimer();

	/**
	 * Cancels task related to this quest timer.
	 */
	public abstract void cancelTask();

	/**
	 * @return the remaining time of the timer, or -1 in case it doesn't exists.
	 */
	public abstract TimeSpan? getRemainingTime();

	public abstract void run();
}

public class TimerHolder<T>: TimerHolder
	where T: notnull
{
	private readonly T _event;
	private readonly TimeSpan _timeInMs;
	private readonly ScheduledFuture _task;
	private readonly IEventTimerEvent<T> _eventScript;
	private readonly IEventTimerCancel<T> _cancelScript;
	private readonly TimerExecutor<T> _postExecutor;
	
	public TimerHolder(T @event, StatSet @params, TimeSpan timeInMs, Npc npc, Player player, bool isRepeating,
		IEventTimerEvent<T> eventScript, IEventTimerCancel<T> cancelScript, TimerExecutor<T> postExecutor)
		: base(@params, npc, player, isRepeating)
	{
		Objects.requireNonNull(@event, GetType().Name + ": \"event\" cannot be null!");
		Objects.requireNonNull(eventScript, GetType().Name + ": \"script\" cannot be null!");
		Objects.requireNonNull(postExecutor, GetType().Name + ": \"postExecutor\" cannot be null!");
		_event = @event;
		_timeInMs = timeInMs;
		_eventScript = eventScript;
		_cancelScript = cancelScript;
		_postExecutor = postExecutor;
		_task = isRepeating
			? ThreadPool.scheduleAtFixedRate(this, _timeInMs, _timeInMs)
			: ThreadPool.schedule(this, _timeInMs);

		if (npc != null)
		{
			npc.addTimerHolder(this);
		}

		if (player != null)
		{
			player.addTimerHolder(this);
		}
	}

	/**
	 * @return the event/key of this timer
	 */
	public T getEvent()
	{
		return _event;
	}

	/**
	 * Cancels this timer.
	 */
	public override void cancelTimer()
	{
		if (getNpc() != null)
		{
			getNpc().removeTimerHolder(this);
		}

		if (getPlayer() != null)
		{
			getPlayer().removeTimerHolder(this);
		}

		if ((_task == null) || _task.isCancelled() || _task.isDone())
		{
			return;
		}

		_task.cancel(true);
		_cancelScript.onTimerCancel(this);
	}

	/**
	 * Cancels task related to this quest timer.
	 */
	public override void cancelTask()
	{
		if ((_task != null) && !_task.isDone() && !_task.isCancelled())
		{
			_task.cancel(false);
		}
	}

	/**
	 * @return the remaining time of the timer, or -1 in case it doesn't exists.
	 */
	public override TimeSpan? getRemainingTime()
	{
		if ((_task == null) || _task.isCancelled() || _task.isDone())
		{
			return null;
		}

		return _task.getDelay();
	}

	/**
	 * @param event
	 * @param npc
	 * @param player
	 * @return {@code true} if event, npc, player are equals to the ones stored in this TimerHolder, {@code false} otherwise
	 */
	public bool isEqual(T @event, Npc npc, Player player)
	{
		return _event.Equals(@event) && (getNpc() == npc) && (getPlayer() == player);
	}

	/**
	 * @param timer the other timer to be compared with.
	 * @return {@code true} of both of timers' npc, event and player match, {@code false} otherwise.
	 */
	public bool isEqual(TimerHolder<T> timer)
	{
		return _event.Equals(timer._event) && (getNpc() == timer.getNpc()) && (getPlayer() == timer.getPlayer());
	}

	public override void run()
	{
		// Notify the post executor to remove this timer from the map
		_postExecutor.onTimerPostExecute(this);

		// Notify the script that the event has been fired.
		_eventScript.onTimerEvent(this);
	}

	public override String ToString()
	{
		return "event: " + _event + " params: " + getParams() + " time: " + _timeInMs + " npc: " + getNpc() + " player: " +
		       getPlayer() + " repeating: " + isRepeating() + " script: " + _eventScript.GetType().Name +
		       " postExecutor: " + _postExecutor.GetType().Name;
	}
}