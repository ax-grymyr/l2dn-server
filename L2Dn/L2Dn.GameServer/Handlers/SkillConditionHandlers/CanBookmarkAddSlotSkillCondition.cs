using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author
 */
public class CanBookmarkAddSlotSkillCondition: ISkillCondition
{
	private readonly int _teleportBookmarkSlots;
	
	public CanBookmarkAddSlotSkillCondition(StatSet @params)
	{
		_teleportBookmarkSlots = @params.getInt("teleportBookmarkSlots");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		Player player = caster.getActingPlayer();
		if (player == null)
		{
			return false;
		}
		
		if ((player.getBookMarkSlot() + _teleportBookmarkSlots) > 30)
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_REACHED_THE_MAXIMUM_NUMBER_OF_MY_TELEPORT_SLOTS_OR_USE_CONDITIONS_ARE_NOT_OBSERVED);
			return false;
		}
		
		return true;
	}
}