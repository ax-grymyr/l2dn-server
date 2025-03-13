using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Berezkin Nikolay
 */
public class PetSkillAcquireHolder
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly int _reqLvl;
	private readonly int _evolve;
	private readonly ItemHolder? _item;

	public PetSkillAcquireHolder(int skillId, int skillLevel, int reqLvl, int evolve, ItemHolder? item)
	{
		_skillId = skillId;
		_skillLevel = skillLevel;
		_reqLvl = reqLvl;
		_evolve = evolve;
		_item = item;
	}

	public int getSkillId()
	{
		return _skillId;
	}

	public int getSkillLevel()
	{
		return _skillLevel;
	}

	public int getReqLvl()
	{
		return _reqLvl;
	}

	public int getEvolve()
	{
		return _evolve;
	}

	public ItemHolder? getItem()
	{
		return _item;
	}
}