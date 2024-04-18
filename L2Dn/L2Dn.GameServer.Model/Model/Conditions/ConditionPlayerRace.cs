using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerRace.
 * @author mkizub, Zoey76
 */
public class ConditionPlayerRace : Condition
{
	private readonly Set<Race> _races;
	
	/**
	 * Instantiates a new condition player race.
	 * @param races the list containing the allowed races.
	 */
	public ConditionPlayerRace(Set<Race> races)
	{
		_races = races;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effector == null) || !effector.isPlayer())
		{
			return false;
		}
		return _races.Contains(effector.getActingPlayer().getRace());
	}
}
