using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasCastle.
 * @author MrPoke
 */
public class ConditionPlayerHasCastle: Condition
{
	private readonly int _castle;
	
	/**
	 * Instantiates a new condition player has castle.
	 * @param castle the castle
	 */
	public ConditionPlayerHasCastle(int castle)
	{
		_castle = castle;
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
			return _castle == 0;
		}
		
		// Any castle
		if (_castle == -1)
		{
			return clan.getCastleId() > 0;
		}
		return clan.getCastleId() == _castle;
	}
}
