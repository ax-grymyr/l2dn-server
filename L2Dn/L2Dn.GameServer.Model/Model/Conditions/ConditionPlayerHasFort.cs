using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasFort.
 * @author MrPoke
 */
public class ConditionPlayerHasFort : Condition
{
	private readonly int _fort;
	
	/**
	 * Instantiates a new condition player has fort.
	 * @param fort the fort
	 */
	public ConditionPlayerHasFort(int fort)
	{
		_fort = fort;
	}
	
	/**
	 * Test impl.
	 * @return true, if successful
	 */
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		
		Clan clan = effector.getActingPlayer().getClan();
		if (clan == null)
		{
			return _fort == 0;
		}
		
		// Any fortress
		if (_fort == -1)
		{
			return clan.getFortId() > 0;
		}
		return clan.getFortId() == _fort;
	}
}
