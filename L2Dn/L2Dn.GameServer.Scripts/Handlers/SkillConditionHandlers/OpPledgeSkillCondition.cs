using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpPledgeSkillCondition: ISkillCondition
{
    private readonly int _level;

    public OpPledgeSkillCondition(StatSet @params)
    {
        _level = @params.getInt("level");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Clan? clan = caster.getClan();
        return clan != null && clan.getLevel() >= _level;
    }
}