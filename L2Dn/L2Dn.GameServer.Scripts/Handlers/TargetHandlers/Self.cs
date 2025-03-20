using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target yourself.
 * @author Nik
 */
public class Self: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.SELF;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		if (creature.isInsideZone(ZoneId.PEACE) && skill.IsBad)
		{
			if (sendMessage)
			{
				creature.sendPacket(SystemMessageId.YOU_CANNOT_USE_SKILLS_THAT_MAY_HARM_OTHER_PLAYERS_IN_HERE);
			}

			return null;
		}
		return creature;
	}
}