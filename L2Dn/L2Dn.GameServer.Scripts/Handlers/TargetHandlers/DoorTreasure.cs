using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target door or treasure chest.
 * @author UnAfraid
 */
public class DoorTreasure: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.DOOR_TREASURE;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		WorldObject? target = creature.getTarget();
		if ((target != null) && (target.isDoor() || (target is Chest)))
		{
			return target;
		}

		if (sendMessage)
		{
			creature.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
		}

		return null;
	}
}