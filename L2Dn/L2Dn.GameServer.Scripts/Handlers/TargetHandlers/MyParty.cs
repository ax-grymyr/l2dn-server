using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Something like target self, but party. Used in aura skills.
 * @author Nik
 */
public class MyParty: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.MY_PARTY;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		if ((selectedTarget != null) && selectedTarget.isPlayer() && (selectedTarget != creature))
		{
			Party party = creature.getParty();
			Party? targetParty = selectedTarget.getActingPlayer()?.getParty();
			if ((party != null) && (targetParty != null) && (party.getLeaderObjectId() == targetParty.getLeaderObjectId()))
			{
				return selectedTarget;
			}
		}
		return creature;
	}
}