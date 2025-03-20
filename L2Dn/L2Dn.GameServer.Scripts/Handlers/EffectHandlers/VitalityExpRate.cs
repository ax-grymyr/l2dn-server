using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("VitalityExpRate")]
public sealed class VitalityExpRate(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.VITALITY_EXP_RATE)
{
    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeMul(Stat.VITALITY_EXP_RATE, Amount / 100 + 1);
        effected.getStat().mergeAdd(Stat.VITALITY_SKILLS, 1d);

        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        player.sendUserBoostStat();
    }
}