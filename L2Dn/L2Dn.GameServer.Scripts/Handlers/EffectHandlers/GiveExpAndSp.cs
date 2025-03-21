using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give XP and SP effect implementation.
/// </summary>
[HandlerStringKey("GiveExpAndSp")]
public sealed class GiveExpAndSp: AbstractEffect
{
    private readonly int _xp;
    private readonly int _sp;

    public GiveExpAndSp(EffectParameterSet parameters)
    {
        _xp = parameters.GetInt32(XmlSkillEffectParameterType.Xp, 0);
        _sp = parameters.GetInt32(XmlSkillEffectParameterType.Sp, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
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