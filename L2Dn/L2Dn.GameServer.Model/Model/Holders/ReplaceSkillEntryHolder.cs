namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Berezkin Nikolay
 */
public class ReplaceSkillEntryHolder
{
	private readonly int _abnormalSkillId;
	private readonly int _abnormalSkillLvl;
	private readonly int _originalSkillId;
	private readonly int _replaceSkillId;

	public ReplaceSkillEntryHolder(int abnormalSkillId, int abnormalSkillLvl, int originalSkillId, int replaceSkillId)
	{
		_abnormalSkillId = abnormalSkillId;
		_abnormalSkillLvl = abnormalSkillLvl;
		_originalSkillId = originalSkillId;
		_replaceSkillId = replaceSkillId;
	}

	public int getAbnormalSkillId()
	{
		return _abnormalSkillId;
	}

	public int getAbnormalSkillLvl()
	{
		return _abnormalSkillLvl;
	}

	public int getOriginalSkillId()
	{
		return _originalSkillId;
	}

	public int getReplaceSkillId()
	{
		return _replaceSkillId;
	}
}