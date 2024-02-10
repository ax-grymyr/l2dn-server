using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionPlayerHasFreeTeleportBookmarkSlots : Condition
{
	private readonly int _teleportBookmarkSlots;
	
	public ConditionPlayerHasFreeTeleportBookmarkSlots(int teleportBookmarkSlots)
	{
		_teleportBookmarkSlots = teleportBookmarkSlots;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			return false;
		}
		
		if ((player.getBookMarkSlot() + _teleportBookmarkSlots) > 18)
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_REACHED_THE_MAXIMUM_NUMBER_OF_MY_TELEPORT_SLOTS_OR_USE_CONDITIONS_ARE_NOT_OBSERVED);
			return false;
		}
		
		return true;
	}
}
