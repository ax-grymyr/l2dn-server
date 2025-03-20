using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets.MagicLamp;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MagicLampExpRate: AbstractStatPercentEffect
{
    public MagicLampExpRate(StatSet @params): base(@params, Stat.MAGIC_LAMP_EXP_RATE)
    {
    }

    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.MAGIC_LAMP_EXP_RATE, Amount);
        if (skill != null)
        {
            effected.getStat().mergeAdd(Stat.LAMP_BONUS_EXP, Amount);
            effected.getStat().mergeAdd(Stat.LAMP_BONUS_BUFFS_COUNT, 1d);
        }

        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        player.sendPacket(new ExMagicLampInfoPacket(player));
    }
}