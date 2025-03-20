using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Bluff effect implementation.
/// </summary>
public sealed class Bluff: AbstractEffect
{
    private readonly int _chance;

    public Bluff(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcProbability(_chance, effector, effected, skill);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        // Headquarters NPC should not rotate
        if (effected.Id == 35062 || effected.isRaid() || effected.isRaidMinion())
        {
            return;
        }

        effected.broadcastPacket(new StartRotationPacket(effected.ObjectId, effected.getHeading(), 1, 65535));
        effected.broadcastPacket(new StopRotationPacket(effected.ObjectId, effector.getHeading(), 65535));
        effected.setHeading(effector.getHeading());
    }

    public override int GetHashCode() => _chance;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._chance);
}