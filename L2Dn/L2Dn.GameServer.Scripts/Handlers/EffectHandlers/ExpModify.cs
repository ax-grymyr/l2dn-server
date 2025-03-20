using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("ExpModify")]
public sealed class ExpModify(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.BONUS_EXP)
{
    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.BONUS_EXP, Amount);
        if (skill != null && skill.IsActive)
        {
            effected.getStat().mergeAdd(Stat.ACTIVE_BONUS_EXP, Amount);
            effected.getStat().mergeAdd(Stat.BONUS_EXP_BUFFS, 1d);
        }
        else
        {
            effected.getStat().mergeAdd(Stat.BONUS_EXP_PASSIVES, 1d);
        }

        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        player.sendUserBoostStat();
    }
}