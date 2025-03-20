using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpPledgeSkillCondition: ISkillCondition
{
    private readonly int _level;

    public OpPledgeSkillCondition(SkillConditionParameterSet parameters)
    {
        _level = parameters.GetInt32(XmlSkillConditionParameterType.Level);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Clan? clan = caster.getClan();
        return clan != null && clan.getLevel() >= _level;
    }
}