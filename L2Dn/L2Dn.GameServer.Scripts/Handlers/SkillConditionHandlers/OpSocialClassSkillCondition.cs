using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpSocialClass")]
public sealed class OpSocialClassSkillCondition: ISkillCondition
{
    private readonly int _socialClass;

    public OpSocialClassSkillCondition(SkillConditionParameterSet parameters)
    {
        _socialClass = parameters.GetInt32(XmlSkillConditionParameterType.SocialClass);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (player == null || player.getClan() == null)
            return false;

        bool isClanLeader = player.isClanLeader();
        if (_socialClass == -1 && !isClanLeader)
            return false;

        return isClanLeader || player.getPledgeType() >= _socialClass;
    }

    public override int GetHashCode() => _socialClass;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._socialClass);
}