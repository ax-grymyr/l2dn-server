using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * TODO: Target while riding wyvern.
 * @author Nik
 */
public class WyvernTarget: ITargetTypeHandler
{
    public TargetType getTargetType() => TargetType.WYVERN_TARGET;

    public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse,
        bool dontMove, bool sendMessage)
    {
        return null;
    }
}