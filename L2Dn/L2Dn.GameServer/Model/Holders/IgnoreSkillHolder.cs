namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class IgnoreSkillHolder: SkillHolder
{
	private int _instances = 1;
	
	public IgnoreSkillHolder(int skillId, int skillLevel): base(skillId, skillLevel)
	{
	}
	
	public IgnoreSkillHolder(SkillHolder holder): base(holder.getSkill())
	{
	}
	
	public int getInstances()
	{
		return _instances;
	}
	
	public int increaseInstances()
	{
		return Interlocked.Increment(ref _instances);
	}
	
	public int decreaseInstances()
	{
		return Interlocked.Decrement(ref _instances);
	}
}