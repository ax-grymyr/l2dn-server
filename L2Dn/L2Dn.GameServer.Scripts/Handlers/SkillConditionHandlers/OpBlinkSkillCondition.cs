using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpBlinkSkillCondition: ISkillCondition
{
    private readonly int _angle;
    private readonly int _range;

    public OpBlinkSkillCondition(SkillConditionParameterSet parameters)
    {
        Position position = parameters.GetEnum<Position>(XmlSkillConditionParameterType.Direction);
        _angle = position switch
        {
            Position.Back => 0,
            Position.Front => 180,
            _ => -1,
        };

        _range = parameters.GetInt32(XmlSkillConditionParameterType.Range);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        double angle = HeadingUtil.ConvertHeadingToDegrees(caster.getHeading());
        double radian = double.DegreesToRadians(angle);
        double course = double.DegreesToRadians(_angle);
        int x1 = (int)(Math.Cos(Math.PI + radian + course) * _range);
        int y1 = (int)(Math.Sin(Math.PI + radian + course) * _range);
        int x = caster.getX() + x1;
        int y = caster.getY() + y1;
        int z = caster.getZ();
        return GeoEngine.getInstance().
            canMoveToTarget(caster.Location.Location3D, new Location3D(x, y, z), caster.getInstanceWorld());
    }
}