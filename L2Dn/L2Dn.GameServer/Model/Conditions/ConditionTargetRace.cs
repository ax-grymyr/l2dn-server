using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetRace.
 * @author Zealar
 */
public class ConditionTargetRace : Condition
{
	private readonly Race _race;
	
	/**
	 * Instantiates a new condition target race.
	 * @param race containing the allowed race.
	 */
	public ConditionTargetRace(Race race)
	{
		_race = race;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return _race == effected.getRace();
	}
}
