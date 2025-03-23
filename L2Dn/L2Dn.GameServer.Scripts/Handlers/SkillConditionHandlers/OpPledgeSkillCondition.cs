using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpPledge")]
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

    public override int GetHashCode() => _level;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._level);
}