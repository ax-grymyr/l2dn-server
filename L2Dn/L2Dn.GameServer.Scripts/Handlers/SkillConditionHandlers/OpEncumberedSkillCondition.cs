using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * TODO: Verify me, also should Quest items be counted?
 * @author UnAfraid
 */
public class OpEncumberedSkillCondition: ISkillCondition
{
    private readonly int _slotsPercent;
    private readonly int _weightPercent;

    public OpEncumberedSkillCondition(StatSet @params)
    {
        _slotsPercent = @params.getInt("slotsPercent");
        _weightPercent = @params.getInt("weightPercent");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || player == null)
            return false;

        int currentSlotsPercent = CalcPercent(player.getInventoryLimit(), player.getInventory().getNonQuestSize());
        int currentWeightPercent = CalcPercent(player.getMaxLoad(), player.getCurrentLoad());
        return currentSlotsPercent >= _slotsPercent && currentWeightPercent >= _weightPercent;
    }

    private static int CalcPercent(int max, int current)
    {
        return 100 - current * 100 / max;
    }
}