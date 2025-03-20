using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("OpSoulMax")]
public sealed class OpSoulMaxSkillCondition: ISkillCondition
{
    private readonly SoulType _type;

    public OpSoulMaxSkillCondition(SkillConditionParameterSet parameters)
    {
        _type = parameters.GetEnum(XmlSkillConditionParameterType.Type, SoulType.LIGHT);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        int maxSouls = (int)caster.getStat().getValue(Stat.MAX_SOULS);
        Player? player = caster.getActingPlayer();
        return caster.isPlayable() && player != null && player.getChargedSouls(_type) < maxSouls;
    }
}