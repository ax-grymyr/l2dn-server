using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("ReuseSkillIdByDamage")]
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

    public ReuseSkillIdByDamage(EffectParameterSet parameters)
    {
        _minAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MinAttackerLevel, 1);
        _maxAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MaxAttackerLevel, int.MaxValue);
        _minDamage = parameters.GetInt32(XmlSkillEffectParameterType.MinDamage, 1);
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
        _hpPercent = parameters.GetInt32(XmlSkillEffectParameterType.HpPercent, 100);
        _skillId = parameters.GetInt32(XmlSkillEffectParameterType.SkillId, 0);
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 0);
        _attackerType = parameters.GetEnum(XmlSkillEffectParameterType.AttackerType, InstanceType.Creature);
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

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageReceived>(this, onDamageReceivedEvent);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
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