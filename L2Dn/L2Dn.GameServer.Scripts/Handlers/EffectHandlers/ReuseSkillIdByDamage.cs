using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ReuseSkillIdByDamage: AbstractEffect
{
    private readonly int _minAttackerLevel;
    private readonly int _maxAttackerLevel;
    private readonly int _minDamage;
    private readonly int _chance;
    private readonly int _hpPercent;
    private readonly int _skillId;
    private readonly int _amount;
    private readonly InstanceType _attackerType;

    public ReuseSkillIdByDamage(StatSet @params)
    {
        _minAttackerLevel = @params.getInt("minAttackerLevel", 1);
        _maxAttackerLevel = @params.getInt("maxAttackerLevel", int.MaxValue);
        _minDamage = @params.getInt("minDamage", 1);
        _chance = @params.getInt("chance", 100);
        _hpPercent = @params.getInt("hpPercent", 100);
        _skillId = @params.getInt("skillId", 0);
        _amount = @params.getInt("amount", 0);
        _attackerType = @params.getEnum("attackerType", InstanceType.Creature);
    }

    private void onDamageReceivedEvent(OnCreatureDamageReceived @event)
    {
        if (@event.isDamageOverTime() || _chance == 0)
            return;

        Creature? attacker = @event.getAttacker();
        if (attacker == null || attacker == @event.getTarget())
            return;

        if (attacker.getLevel() < _minAttackerLevel || attacker.getLevel() > _maxAttackerLevel)
            return;

        if (@event.getDamage() < _minDamage)
            return;

        if (_chance < 100 && Rnd.get(100) > _chance)
            return;

        if (_hpPercent < 100 && attacker.getCurrentHpPercent() > _hpPercent)
            return;

        if (!attacker.InstanceType.IsType(_attackerType))
            return;

        Player player = (Player)@event.getTarget();
        Skill? s = player.getKnownSkill(_skillId);
        if (s != null)
        {
            if (_amount > 0)
            {
                TimeSpan reuse = player.getSkillRemainingReuseTime(s.ReuseHashCode);
                if (reuse > TimeSpan.Zero)
                {
                    TimeSpan diff = reuse - TimeSpan.FromMilliseconds(_amount);
                    diff = Algorithms.Max(diff, TimeSpan.Zero);

                    player.removeTimeStamp(s);
                    player.addTimeStamp(s, diff);
                    player.sendPacket(new SkillCoolTimePacket(player));
                }
            }
            else
            {
                player.removeTimeStamp(s);
                player.enableSkill(s);
                player.sendPacket(new SkillCoolTimePacket(player));
            }
        }
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageReceived>(this, onDamageReceivedEvent);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageReceived>(onDamageReceivedEvent);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_minAttackerLevel, _maxAttackerLevel, _minDamage, _chance, _hpPercent, _skillId, _amount,
            _attackerType);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._minAttackerLevel, x._maxAttackerLevel, x._minDamage, x._chance, x._hpPercent,
            x._skillId, x._amount, x._attackerType));
}