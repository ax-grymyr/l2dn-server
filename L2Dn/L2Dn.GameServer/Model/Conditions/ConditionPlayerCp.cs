using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerCp.
 */
public class ConditionPlayerCp: Condition
{
	private readonly int _cp;
	
	/**
	 * Instantiates a new condition player cp.
	 * @param cp the cp
	 */
	public ConditionPlayerCp(int cp)
	{
		_cp = cp;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector != null) && (((effector.getCurrentCp() * 100) / effector.getMaxCp()) >= _cp);
	}
}
