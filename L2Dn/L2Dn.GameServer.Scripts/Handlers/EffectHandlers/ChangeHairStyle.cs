using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Change Hair Style effect implementation.
/// </summary>
public sealed class ChangeHairStyle: AbstractEffect
{
    private readonly int _value;

    public ChangeHairStyle(StatSet @params)
    {
        _value = @params.getInt("value", 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        player.getAppearance().setHairStyle(_value);
        player.broadcastUserInfo();
    }

    public override int GetHashCode() => _value;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._value);
}