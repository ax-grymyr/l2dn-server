using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class CheckSexSkillCondition: ISkillCondition
{
    private readonly Sex _sex;

    public CheckSexSkillCondition(StatSet @params)
    {
        _sex = @params.getBoolean("isFemale") ? Sex.Female : Sex.Male;
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        return caster.isPlayer() && player != null && player.getAppearance().getSex() == _sex;
    }
}