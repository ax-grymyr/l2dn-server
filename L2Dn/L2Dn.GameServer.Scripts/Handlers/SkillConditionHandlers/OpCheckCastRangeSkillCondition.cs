using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCheckCastRange")]
public sealed class OpCheckCastRangeSkillCondition: ISkillCondition
{
    private readonly int _distance;

    public OpCheckCastRangeSkillCondition(SkillConditionParameterSet parameters)
    {
        _distance = parameters.GetInt32(XmlSkillConditionParameterType.Distance);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return target != null
            && caster.Distance3D(target) >= _distance
            && GeoEngine.getInstance().canSeeTarget(caster, target);
    }

    public override int GetHashCode() => _distance;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._distance);
}