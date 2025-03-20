using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Focus Souls effect implementation.
/// </summary>
public sealed class FocusSouls: AbstractEffect
{
    private readonly int _charge;
    private readonly SoulType _type;

    public FocusSouls(StatSet @params)
    {
        _charge = @params.getInt("charge", 0);
        _type = @params.getEnum("type", SoulType.LIGHT);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? target = effected.getActingPlayer();
        if (!effected.isPlayer() || effected.isAlikeDead() || target == null)
            return;

        int maxSouls = (int)target.getStat().getValue(Stat.MAX_SOULS, 0);
        if (maxSouls > 0)
        {
            int amount = _charge;
            if (target.getChargedSouls(_type) < maxSouls)
            {
                int count = target.getChargedSouls(_type) + amount <= maxSouls
                    ? amount
                    : maxSouls - target.getChargedSouls(_type);

                target.increaseSouls(count, _type);
            }
            else
            {
                target.sendPacket(SystemMessageId.SOUL_CANNOT_BE_INCREASED_ANYMORE);
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_charge, _type);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._charge, x._type));
}