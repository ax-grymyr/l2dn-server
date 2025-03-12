using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class VitalityExpRate(StatSet @params): AbstractStatPercentEffect(@params, Stat.VITALITY_EXP_RATE)
{
    public override void pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeMul(Stat.VITALITY_EXP_RATE, Amount / 100 + 1);
        effected.getStat().mergeAdd(Stat.VITALITY_SKILLS, 1d);

        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        player.sendUserBoostStat();
    }
}