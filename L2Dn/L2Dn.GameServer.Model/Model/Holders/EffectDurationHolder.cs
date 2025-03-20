using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Holders;

/**
 * Effect duration holder.
 * @author Zoey76
 */
public class EffectDurationHolder
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly int _duration;
	
	/**
	 * Effect duration holder constructor.
	 * @param skill the skill to get the data
	 * @param duration the effect duration
	 */
	public EffectDurationHolder(Skill skill, int duration)
	{
		_skillId = skill.DisplayId;
		_skillLevel = skill.DisplayLevel;
		_duration = duration;
	}
	
	/**
	 * Get the effect's skill Id.
	 * @return the skill Id
	 */
	public int getSkillId()
	{
		return _skillId;
	}
	
	/**
	 * Get the effect's skill level.
	 * @return the skill level
	 */
	public int getSkillLevel()
	{
		return _skillLevel;
	}
	
	/**
	 * Get the effect's duration.
	 * @return the duration in <b>seconds</b>
	 */
	public int getDuration()
	{
		return _duration;
	}
}