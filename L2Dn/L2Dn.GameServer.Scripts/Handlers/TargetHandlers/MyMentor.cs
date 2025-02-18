using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target my mentor.
 * @author Nik
 */
public class MyMentor: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.MY_MENTOR;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		if (creature.isPlayer())
		{
			Mentee? mentor = MentorManager.getInstance().getMentor(creature.ObjectId);
			if (mentor != null)
			{
				return mentor.getPlayer();
			}
		}
		return null;
	}
}