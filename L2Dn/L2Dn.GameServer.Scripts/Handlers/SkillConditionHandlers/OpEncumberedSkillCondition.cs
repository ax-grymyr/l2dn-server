using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

// TODO: Verify me, also should Quest items be counted?
[HandlerName("OpEncumbered")]
public sealed class OpEncumberedSkillCondition: ISkillCondition
{
    private readonly int _slotsPercent;
    private readonly int _weightPercent;

    public OpEncumberedSkillCondition(SkillConditionParameterSet parameters)
    {
        _slotsPercent = parameters.GetInt32(XmlSkillConditionParameterType.SlotsPercent);
        _weightPercent = parameters.GetInt32(XmlSkillConditionParameterType.WeightPercent);
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