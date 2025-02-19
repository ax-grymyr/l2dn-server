using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class TargetMyPartySkillCondition: ISkillCondition
{
    private readonly bool _includeMe;

    public TargetMyPartySkillCondition(StatSet @params)
    {
        _includeMe = @params.getBoolean("includeMe");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if ((target == null) || !target.isPlayer())
        {
            return false;
        }

        Party? party = caster.getParty();
        Party? targetParty = target.getActingPlayer()?.getParty();
        return ((party == null)
            ? (_includeMe && (caster == target))
            : (_includeMe ? party == targetParty : (party == targetParty) && (caster != target)));
    }
}