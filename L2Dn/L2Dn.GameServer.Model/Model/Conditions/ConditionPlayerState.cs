using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerState.
 * @author mkizub
 */
public class ConditionPlayerState : Condition
{
	private readonly PlayerState _check;
	private readonly bool _required;
	
	/**
	 * Instantiates a new condition player state.
	 * @param check the player state to be verified.
	 * @param required the required value.
	 */
	public ConditionPlayerState(PlayerState check, bool required)
	{
		_check = check;
		_required = required;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		switch (_check)
		{
			case PlayerState.RESTING:
			{
				if (player != null)
				{
					return (player.isSitting() == _required);
				}
				return !_required;
			}
			case PlayerState.MOVING:
			{
				return effector.isMoving() == _required;
			}
			case PlayerState.RUNNING:
			{
				return effector.isRunning() == _required;
			}
			case PlayerState.STANDING:
			{
				if (player != null)
				{
					return (_required != (player.isSitting() || player.isMoving()));
				}
				return (_required != effector.isMoving());
			}
			case PlayerState.FLYING:
			{
				return (effector.isFlying() == _required);
			}
			case PlayerState.BEHIND:
			{
				return (((ILocational)effector).isBehind(effected) == _required);
			}
			case PlayerState.FRONT:
			{
				return (((ILocational)effector).isInFrontOf(effected) == _required);
			}
			case PlayerState.CHAOTIC:
			{
				if (player != null)
				{
					return ((player.getReputation() < 0) == _required);
				}
				return !_required;
			}
			case PlayerState.OLYMPIAD:
			{
				if (player != null)
				{
					return (player.isInOlympiadMode() == _required);
				}
				return !_required;
			}
		}
		return !_required;
	}
}
