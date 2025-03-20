using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * @author Mobius
 */
public class OwnerPet: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.OWNER_PET;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		return creature.getActingPlayer();
	}
}