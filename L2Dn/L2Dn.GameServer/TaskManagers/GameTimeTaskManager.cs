using System.Globalization;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl;

namespace L2Dn.GameServer.TaskManagers;

/**
 * GameTime task manager class.
 * @author Forsaiken, Mobius
 */
public class GameTimeTaskManager
{
	public const int TICKS_PER_SECOND = 10; // Not able to change this without checking through code.
	public const int MILLIS_IN_TICK = 1000 / TICKS_PER_SECOND;
	public const int IG_DAYS_PER_DAY = 6;
	public const int MILLIS_PER_IG_DAY = (3600000 * 24) / IG_DAYS_PER_DAY;
	public const int SECONDS_PER_IG_DAY = MILLIS_PER_IG_DAY / 1000;
	public const int TICKS_PER_IG_DAY = SECONDS_PER_IG_DAY * TICKS_PER_SECOND;
	
	private readonly DateTime _referenceTime;
	private bool _isNight;
	private int _gameTicks;
	private int _gameTime;
	private int _gameHour;
	private bool _stopRequested;
	
	public GameTimeTaskManager()
	{
		//base.setPriority(MAX_PRIORITY); // Max priority thread

		DateTime now = DateTime.Today;
		_referenceTime = now;

		System.Threading.Tasks.Task.Run(() => run()); // TODO: run as task for now 
	}
	
	public void stop()
	{
		_stopRequested = true;
	}

	private async void run()
	{
		while (!_stopRequested)
		{
			_gameTicks = (int)((DateTime.Now - _referenceTime).TotalMilliseconds / MILLIS_IN_TICK);
			_gameTime = (_gameTicks % TICKS_PER_IG_DAY) / MILLIS_IN_TICK;
			_gameHour = _gameTime / 60;
			
			if ((_gameHour < 6) != _isNight)
			{
				_isNight = !_isNight;
				
				if (EventDispatcher.getInstance().hasListener(EventType.ON_DAY_NIGHT_CHANGE))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnDayNightChange(_isNight));
				}
			}
			
			try
			{
				await System.Threading.Tasks.Task.Delay(MILLIS_IN_TICK);
			}
			catch (TaskCanceledException)
			{
				// Ignore.
			}
		}
	}
	
	public bool isNight()
	{
		return _isNight;
	}
	
	/**
	 * @return The actual GameTime tick. Directly taken from current time.
	 */
	public int getGameTicks()
	{
		return _gameTicks;
	}
	
	public int getGameTime()
	{
		return _gameTime;
	}
	
	public int getGameHour()
	{
		return _gameHour;
	}
	
	public int getGameMinute()
	{
		return _gameTime % 60;
	}
	
	public static GameTimeTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly GameTimeTaskManager INSTANCE = new GameTimeTaskManager();
	}
}