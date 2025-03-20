using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model;

/**
 * Simple class containing all necessary information to maintain<br>
 * valid time stamps and reuse for skills and items reuse upon re-login.<br>
 * <b>Filter this carefully as it becomes redundant to store reuse for small delays.</b>
 * @author Yesod, Zoey76, Mobius
 */
public sealed class TimeStamp
{
	/** Item or skill ID. */
	private readonly int _id1;
	/** Item object ID or skill level. */
	private readonly int _id2;
	/** Skill level. */
	private readonly int _id3;
	/** Item or skill reuse time. */
	private readonly TimeSpan _reuse;
	/** Time stamp. */
	private DateTime? _stamp;
	/** Shared reuse group. */
	private readonly int _group;

	/**
	 * Skill time stamp constructor.
	 * @param skill the skill upon the stamp will be created.
	 * @param reuse the reuse time for this skill.
	 * @param systime overrides the system time with a customized one.
	 */
	public TimeStamp(Skill skill, TimeSpan reuse, DateTime? systime = null)
	{
		_id1 = skill.Id;
		_id2 = skill.Level;
		_id3 = skill.SubLevel;
		_reuse = reuse;
		_stamp = systime != null ? systime.Value : reuse > TimeSpan.Zero ? DateTime.UtcNow + reuse : null;
		_group = skill.ReuseDelayGroup;
	}

	/**
	 * Item time stamp constructor.
	 * @param item the item upon the stamp will be created.
	 * @param reuse the reuse time for this item.
	 * @param systime overrides the system time with a customized one.
	 */
	public TimeStamp(Item item, TimeSpan reuse, DateTime? systime)
	{
		_id1 = item.Id;
		_id2 = item.ObjectId;
		_id3 = 0;
		_reuse = reuse;
		_stamp = systime != null ? systime.Value : reuse > TimeSpan.Zero ? DateTime.UtcNow + reuse : null;
		_group = item.getSharedReuseGroup();
	}

	/**
	 * Gets the time stamp.
	 * @return the time stamp, either the system time where this time stamp was created or the custom time assigned
	 */
	public DateTime? getStamp()
	{
		return _stamp;
	}

	/**
	 * Gets the item ID.
	 * @return the item ID
	 */
	public int getItemId()
	{
		return _id1;
	}

	/**
	 * Gets the item object ID.
	 * @return the item object ID
	 */
	public int getItemObjectId()
	{
		return _id2;
	}

	/**
	 * Gets the skill ID.
	 * @return the skill ID
	 */
	public int getSkillId()
	{
		return _id1;
	}

	/**
	 * Gets the skill level.
	 * @return the skill level
	 */
	public int getSkillLevel()
	{
		return _id2;
	}

	/**
	 * Gets the skill sub level.
	 * @return the skill level
	 */
	public int getSkillSubLevel()
	{
		return _id3;
	}

	/**
	 * Gets the reuse.
	 * @return the reuse
	 */
	public TimeSpan getReuse()
	{
		return _reuse;
	}

	/**
	 * Get the shared reuse group.<br>
	 * Only used on items.
	 * @return the shared reuse group
	 */
	public int getSharedReuseGroup()
	{
		return _group;
	}

	/**
	 * Gets the remaining time.
	 * @return the remaining time for this time stamp to expire
	 */
	public TimeSpan getRemaining()
	{
		if (_stamp == null)
		{
			return TimeSpan.Zero;
		}

		TimeSpan remainingTime = _stamp.Value - DateTime.UtcNow;
		if (remainingTime <= TimeSpan.Zero)
		{
			_stamp = null;
			remainingTime = TimeSpan.Zero;
		}

		return remainingTime;
	}

	/**
	 * Verifies if the reuse delay has passed.
	 * @return {@code true} if this time stamp has expired, {@code false} otherwise
	 */
	public bool hasNotPassed()
	{
		if (_stamp == null)
		{
			return false;
		}

		bool hasNotPassed = DateTime.UtcNow < _stamp.Value;
		if (!hasNotPassed)
		{
			_stamp = null;
		}

		return hasNotPassed;
	}
}