using L2Dn.GameServer.AI;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Spoil effect implementation.
/// </summary>
[HandlerName("Spoil")]
public sealed class Spoil: AbstractEffect
{
    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcMagicSuccess(effector, effected, skill);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isMonster() || effected.isDead())
        {
            effector.sendPacket(SystemMessageId.INVALID_TARGET);
            return;
        }

        Monster target = (Monster)effected;
        if (target.isSpoiled())
        {
            effector.sendPacket(SystemMessageId.IT_HAS_ALREADY_BEEN_SPOILED);
            return;
        }

        target.setSpoilerObjectId(effector.ObjectId);
        effector.sendPacket(SystemMessageId.THE_SPOIL_CONDITION_HAS_BEEN_ACTIVATED);
        target.getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, effector);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}