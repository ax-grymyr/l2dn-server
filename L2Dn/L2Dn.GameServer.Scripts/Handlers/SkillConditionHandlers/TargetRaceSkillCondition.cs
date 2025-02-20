using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid, Mobius
 */
public class TargetRaceSkillCondition: ISkillCondition
{
    private readonly Set<Race> _races = [];

    public TargetRaceSkillCondition(StatSet @params)
    {
        List<Race> races = @params.getEnumList<Race>("race");
        if (races != null)
        {
            _races.addAll(races);
        }
        else
        {
            _races.add(@params.getEnum<Race>("race"));
        }
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isCreature())
        {
            return false;
        }

        return _races.Contains(((Creature)target).getRace());
    }
}