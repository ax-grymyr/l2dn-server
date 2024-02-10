using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Call Pc condition implementation.
 * @author Adry_85
 */
public class ConditionPlayerCallPc: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerCallPc(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool canCallPlayer = true;
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			canCallPlayer = false;
		}
		else if (player.isInOlympiadMode())
		{
			player.sendPacket(SystemMessageId.CANNOT_BE_SUMMONED_IN_THIS_LOCATION);
			canCallPlayer = false;
		}
		else if (player.inObserverMode())
		{
			canCallPlayer = false;
		}
		else if (player.isInsideZone(ZoneId.NO_SUMMON_FRIEND) || player.isInsideZone(ZoneId.JAIL) || player.isFlyingMounted())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_SUMMONING_OR_TELEPORTING_IN_THIS_AREA);
			canCallPlayer = false;
		}
		return (_value == canCallPlayer);
	}
}
