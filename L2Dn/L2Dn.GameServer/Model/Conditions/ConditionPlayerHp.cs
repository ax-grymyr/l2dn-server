using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHp.
 * @author mr
 */
public class ConditionPlayerHp : Condition
{
	private readonly int _hp;
	
	/**
	 * Instantiates a new condition player hp.
	 * @param hp the hp
	 */
	public ConditionPlayerHp(int hp)
	{
		_hp = hp;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector != null) && (((effector.getCurrentHp() * 100) / effector.getMaxHp()) <= _hp);
	}
}
