using System.Runtime.CompilerServices;

namespace L2Dn.GameServer.Model.Stats;

/**
 * A class representing the basic property resist of mesmerizing debuffs.
 * @author Nik
 */
public class BasicPropertyResist
{
	private static readonly TimeSpan RESIST_DURATION = TimeSpan.FromSeconds(15); // The resistance stays no longer than 15 seconds after last mesmerizing debuff.
	
	private DateTime _resistanceEndTime = DateTime.MinValue;
	private volatile int _resistanceLevel;
	
	/**
	 * Checks if the resist has expired.
	 * @return {@code true} if it has expired, {@code false} otherwise
	 */
	public bool isExpired()
	{
		return DateTime.Now > _resistanceEndTime;
	}
	
	/**
	 * Gets the remain time.
	 * @return the remain time
	 */
	public TimeSpan getRemainTime()
	{
		return DateTime.Now - _resistanceEndTime;
	}
	
	/**
	 * Gets the resist level.
	 * @return the resist level
	 */
	public int getResistLevel()
	{
		return !isExpired() ? _resistanceLevel : 0;
	}
	
	/**
	 * Increases the resist level while checking if the resist has expired so it starts counting it from 1.
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void increaseResistLevel()
	{
		// Check if the level needs to be reset due to timer warn off.
		if (isExpired())
		{
			_resistanceLevel = 1;
			_resistanceEndTime = DateTime.Now + RESIST_DURATION;
		}
		else
		{
			_resistanceLevel++;
		}
	}
}