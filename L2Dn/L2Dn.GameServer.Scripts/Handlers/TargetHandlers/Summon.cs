using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target automatically one of my summons.
 * @author Nik
 */
public class Summon: ITargetTypeHandler
{
    public TargetType getTargetType()
    {
        return TargetType.SUMMON;
    }

    public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse,
        bool dontMove, bool sendMessage)
    {
        if (creature.isPlayer() && creature.hasSummon())
        {
            return creature.getActingPlayer()?.getAnyServitor();
        }

        return creature.getPet();
    }
}