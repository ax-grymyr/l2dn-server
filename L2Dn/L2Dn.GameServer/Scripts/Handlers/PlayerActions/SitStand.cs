using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.PlayerActions;

/**
 * Sit/Stand player action handler.
 * @author UnAfraid
 */
public class SitStand: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (player.isSitting() || !player.isMoving() || player.isFakeDeath())
		{
			useSit(player, player.getTarget());
		}
		else
		{
			// Sit when arrive using next action.
			// Creating next action class.
			NextAction nextAction = new NextAction(CtrlEvent.EVT_ARRIVED, CtrlIntention.AI_INTENTION_MOVE_TO,
				new Callback(() => useSit(player, player.getTarget())));
			
			// Binding next action to AI.
			player.getAI().setNextAction(nextAction);
		}
	}

	public bool isPetAction()
	{
		return false;
	}

	private sealed class Callback(Action action): NextAction.NextActionCallback
	{
		public void doWork() => action();
	}
	
	/**
	 * Use the sit action.
	 * @param player the player trying to sit
	 * @param target the target to sit, throne, bench or chair
	 * @return {@code true} if the player can sit, {@code false} otherwise
	 */
	private bool useSit(Player player, WorldObject target)
	{
		if (player.getMountType() != MountType.NONE)
		{
			return false;
		}
		
		if (!player.isSitting() && (target is StaticObject) && (((StaticObject) target).getType() == 1) && player.isInsideRadius2D(target, StaticObject.INTERACTION_DISTANCE))
		{
			ChairSitPacket cs = new ChairSitPacket(player, target.getId());
			player.sendPacket(cs);
			player.sitDown();
			player.broadcastPacket(cs);
			return true;
		}
		
		if (player.isFakeDeath())
		{
			player.stopEffects(EffectFlag.FAKE_DEATH);
		}
		else if (player.isSitting())
		{
			player.standUp();
		}
		else
		{
			player.sitDown();
		}
		return true;
	}
}