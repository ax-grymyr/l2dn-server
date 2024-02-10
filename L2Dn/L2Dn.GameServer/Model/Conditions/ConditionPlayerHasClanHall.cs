using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasClanHall.
 * @author MrPoke
 */
public class ConditionPlayerHasClanHall: Condition
{
	private readonly List<int> _clanHall;
	
	/**
	 * Instantiates a new condition player has clan hall.
	 * @param clanHall the clan hall
	 */
	public ConditionPlayerHasClanHall(List<int> clanHall)
	{
		_clanHall = clanHall;
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
			return ((_clanHall.Count == 1) && (_clanHall[0] == 0));
		}
		
		// All Clan Hall
		if ((_clanHall.Count == 1) && (_clanHall[0] == -1))
		{
			return clan.getHideoutId() > 0;
		}
		return _clanHall.Contains(clan.getHideoutId());
	}
}
