namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Nik
 */
public class AttachSkillHolder: SkillHolder
{
	private readonly int _requiredSkillId;
	private readonly int _requiredSkillLevel;

	public AttachSkillHolder(int skillId, int skillLevel, int requiredSkillId, int requiredSkillLevel): base(skillId,
		skillLevel)
	{
		_requiredSkillId = requiredSkillId;
		_requiredSkillLevel = requiredSkillLevel;
	}

	public int getRequiredSkillId()
	{
		return _requiredSkillId;
	}

	public int getRequiredSkillLevel()
	{
		return _requiredSkillLevel;
	}

	public static AttachSkillHolder fromStatSet(StatSet set)
	{
		return new AttachSkillHolder(set.getInt("skillId"), set.getInt("skillLevel", 1), set.getInt("requiredSkillId"),
			set.getInt("requiredSkillLevel", 1));
	}
}