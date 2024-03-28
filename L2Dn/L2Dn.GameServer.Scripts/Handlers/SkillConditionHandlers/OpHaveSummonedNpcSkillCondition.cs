using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid, Mobius
 */
public class OpHaveSummonedNpcSkillCondition: ISkillCondition
{
	private readonly int _npcId;
	
	public OpHaveSummonedNpcSkillCondition(StatSet @params)
	{
		_npcId = @params.getInt("npcId");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		foreach (Npc npc in caster.getSummonedNpcs())
		{
			if (npc.getId() == _npcId)
			{
				return true;
			}
		}
		return false;
	}
}