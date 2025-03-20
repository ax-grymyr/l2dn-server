using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger heal percent by skill effect implementation.
/// </summary>
public sealed class TriggerHealPercentBySkill: AbstractEffect
{
    private readonly int _castSkillId;
    private readonly int _chance;
    private readonly int _power;

    public TriggerHealPercentBySkill(EffectParameterSet parameters)
    {
        _castSkillId = parameters.GetInt32(XmlSkillEffectParameterType.CastSkillId);
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
        _power = parameters.GetInt32(XmlSkillEffectParameterType.Power, 0);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_chance == 0 || _castSkillId == 0)
            return;

        effected.Events.Subscribe<OnCreatureSkillFinishCast>(this, OnSkillUseEvent);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureSkillFinishCast>(OnSkillUseEvent);
    }

    public override EffectTypes EffectTypes => EffectTypes.HEAL;

    private void OnSkillUseEvent(OnCreatureSkillFinishCast @event)
    {
        if (_castSkillId != @event.getSkill().Id)
            return;

        WorldObject? target = @event.getTarget();
        if (target == null)
            return;

        Player? player = target.getActingPlayer();
        if (player == null || player.isDead() || player.isHpBlocked())
            return;

        if (_chance < 100 && Rnd.get(100) > _chance)
            return;

        double power = _power;
        bool full = power == 100.0; // TODO: rounding error?

        double amount = full ? player.getMaxHp() : player.getMaxHp() * power / 100.0;

        // Prevents overheal.
        amount = Math.Min(amount, Math.Max(0, player.getMaxRecoverableHp() - player.getCurrentHp()));
        if (amount >= 0)
        {
            if (amount != 0)
            {
                player.setCurrentHp(amount + player.getCurrentHp(), false);
                player.broadcastStatusUpdate(player);
            }

            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECOVERED_S1_HP);
            sm.Params.addInt((int)amount);
            player.sendPacket(sm);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_castSkillId, _chance, _power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._castSkillId, x._chance, x._power));
}