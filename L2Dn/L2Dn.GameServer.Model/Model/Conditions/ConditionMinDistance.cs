using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Mobius
 */
public sealed class ConditionMinDistance(int distance): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        return effected != null
            && effector.Distance3D(effected) >= distance
            && GeoEngine.getInstance().canSeeTarget(effector, effected);
    }
}