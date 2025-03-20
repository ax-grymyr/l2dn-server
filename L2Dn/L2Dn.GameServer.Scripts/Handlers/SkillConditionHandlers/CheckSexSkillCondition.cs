using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class CheckSexSkillCondition: ISkillCondition
{
    private readonly Sex _sex;

    public CheckSexSkillCondition(SkillConditionParameterSet parameters)
    {
        _sex = parameters.GetBoolean(XmlSkillConditionParameterType.IsFemale) ? Sex.Female : Sex.Male;
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        return caster.isPlayer() && player != null && player.getAppearance().getSex() == _sex;
    }
}