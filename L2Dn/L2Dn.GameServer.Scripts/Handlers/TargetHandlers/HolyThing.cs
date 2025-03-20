using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target siege artefact.
 * @author Nik
 */
public class HolyThing: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.HOLYTHING;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		WorldObject? target = creature.getTarget();
		if ((target != null) && target.isArtefact())
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