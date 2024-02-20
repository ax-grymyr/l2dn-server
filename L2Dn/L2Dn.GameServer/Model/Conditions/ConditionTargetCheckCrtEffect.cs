using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionTargetCheckCrtEffect : Condition
{
	private readonly bool _isCrtEffect;
	
	public ConditionTargetCheckCrtEffect(bool isCrtEffect)
	{
		_isCrtEffect = isCrtEffect;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effected.isNpc())
		{
			return ((Npc) effected).getTemplate().canBeCrt() == _isCrtEffect;
		}
		return true;
	}
}
