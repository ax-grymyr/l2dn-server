using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Events.Timers;

public class TimerHolder<T>: Runnable
{
	private readonly T _event;
	private readonly  StatSet _params;
	private readonly  int _timeInMs;
	private readonly  Npc _npc;
	private readonly  Player _player;
	private readonly  bool _isRepeating;
	private readonly  IEventTimerEvent<T> _eventScript;
	private readonly  IEventTimerCancel<T> _cancelScript;
	private readonly  TimerExecutor<T> _postExecutor;
	private readonly  ScheduledFuture _task;
	
	public TimerHolder(T @event, StatSet @params, int timeInMs, Npc npc, Player player, bool isRepeating, IEventTimerEvent<T> eventScript, IEventTimerCancel<T> cancelScript, TimerExecutor<T> postExecutor)
	{
		Objects.requireNonNull(@event, GetType().Name + ": \"event\" cannot be null!");
		Objects.requireNonNull(eventScript, GetType().Name + ": \"script\" cannot be null!");
		Objects.requireNonNull(postExecutor, GetType().Name + ": \"postExecutor\" cannot be null!");
		_event = @event;
		_params = @params;
		_timeInMs = timeInMs;
		_npc = npc;
		_player = player;
		_isRepeating = isRepeating;
		_eventScript = eventScript;
		_cancelScript = cancelScript;
		_postExecutor = postExecutor;
		_task = isRepeating ? ThreadPool.scheduleAtFixedRate(this, _timeInMs, _timeInMs) : ThreadPool.schedule(this, _timeInMs);
		
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
	public void cancelTimer()
	{
		if (_npc != null)
		{
			_npc.removeTimerHolder(this);
		}
		
		if (_player != null)
		{
			_player.removeTimerHolder(this);
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
	public void cancelTask()
	{
		if ((_task != null) && !_task.isDone() && !_task.isCancelled())
		{
			_task.cancel(false);
		}
	}
	
	/**
	 * @return the remaining time of the timer, or -1 in case it doesn't exists.
	 */
	public long getRemainingTime()
	{
		if ((_task == null) || _task.isCancelled() || _task.isDone())
		{
			return -1;
		}
		return _task.getDelay(TimeUnit.MILLISECONDS);
	}
	
	/**
	 * @param event
	 * @param npc
	 * @param player
	 * @return {@code true} if event, npc, player are equals to the ones stored in this TimerHolder, {@code false} otherwise
	 */
	public bool isEqual(T @event, Npc npc, Player player)
	{
		return _event.Equals(@event) && (_npc == npc) && (_player == player);
	}
	
	/**
	 * @param timer the other timer to be compared with.
	 * @return {@code true} of both of timers' npc, event and player match, {@code false} otherwise.
	 */
	public bool isEqual(TimerHolder<T> timer)
	{
		return _event.Equals(timer._event) && (_npc == timer._npc) && (_player == timer._player);
	}
	
	public void run()
	{
		// Notify the post executor to remove this timer from the map
		_postExecutor.onTimerPostExecute(this);
		
		// Notify the script that the event has been fired.
		_eventScript.onTimerEvent(this);
	}
	
	public override String ToString()
	{
		return "event: " + _event + " params: " + _params + " time: " + _timeInMs + " npc: " + _npc + " player: " + _player + " repeating: " + _isRepeating + " script: " + _eventScript.getClass().getSimpleName() + " postExecutor: " + _postExecutor.getClass().getSimpleName();
	}
}