using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Escape condition implementation.
 * @author Adry_85
 */
public class ConditionPlayerCanEscape: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerCanEscape(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool canTeleport = true;
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			canTeleport = false;
		}
		else if (player.isInDuel())
		{
			canTeleport = false;
		}
		else if (player.isControlBlocked())
		{
			canTeleport = false;
		}
		else if (player.isCombatFlagEquipped())
		{
			canTeleport = false;
		}
		else if (player.isFlying() || player.isFlyingMounted())
		{
			canTeleport = false;
		}
		else if (player.isInOlympiadMode())
		{
			canTeleport = false;
		}
		else if (player.isOnEvent())
		{
			canTeleport = false;
		}
		return (_value == canTeleport);
	}
}