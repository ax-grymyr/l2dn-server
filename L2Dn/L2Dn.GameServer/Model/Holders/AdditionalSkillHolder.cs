namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class AdditionalSkillHolder: SkillHolder
{
	private readonly int _minLevel;
	
	public AdditionalSkillHolder(int skillId, int skillLevel, int minLevel): base(skillId, skillLevel)
	{
		_minLevel = minLevel;
	}
	
	public int getMinLevel()
	{
		return _minLevel;
	}
}