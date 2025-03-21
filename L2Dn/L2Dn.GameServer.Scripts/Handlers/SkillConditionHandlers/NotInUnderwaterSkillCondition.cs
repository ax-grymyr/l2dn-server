using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("NotInUnderwater")]
public sealed class NotInUnderwaterSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return !caster.isInsideZone(ZoneId.WATER);
    }
}