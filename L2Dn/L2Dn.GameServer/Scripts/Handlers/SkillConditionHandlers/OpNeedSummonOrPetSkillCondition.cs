using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid, Mobius
 */
public class OpNeedSummonOrPetSkillCondition: ISkillCondition
{
	private readonly Set<int> _npcIds = new();
	
	public OpNeedSummonOrPetSkillCondition(StatSet @params)
	{
		List<int> npcIds = @params.getList<int>("npcIds");
		if (npcIds != null)
		{
			_npcIds.addAll(npcIds);
		}
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		Summon pet = caster.getPet();
		if ((pet != null) && _npcIds.Contains(pet.getId()))
		{
			return true;
		}
		
		foreach (Summon summon in caster.getServitors().values())
		{
			if (_npcIds.Contains(summon.getId()))
			{
				return true;
			}
		}
		
		return false;
	}
}