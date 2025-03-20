using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give XP and SP effect implementation.
/// </summary>
public sealed class GiveExpAndSp: AbstractEffect
{
    private readonly int _xp;
    private readonly int _sp;

    public GiveExpAndSp(StatSet @params)
    {
        _xp = @params.getInt("xp", 0);
        _sp = @params.getInt("sp", 0);
    }

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null || !effected.isPlayer() || effected.isAlikeDead())
            return;

        if (_sp != 0 && _xp != 0)
        {
            player.getStat().addExp(_xp);
            player.getStat().addSp(_sp);

            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1_XP_BONUS_S2_AND_S3_SP_BONUS_S4);

            sm.Params.addLong(_xp);
            sm.Params.addLong(0);
            sm.Params.addLong(_sp);
            sm.Params.addLong(0);
            effector.sendPacket(sm);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_xp, _sp);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._xp, x._sp));
}