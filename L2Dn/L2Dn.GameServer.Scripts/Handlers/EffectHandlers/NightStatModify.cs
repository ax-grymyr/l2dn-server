using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("NightStatModify")]
public sealed class NightStatModify: AbstractEffect
{
    private static readonly Set<Creature> _nightStatCharacters = [];
    private const int _shadowSense = 294;

    private static bool _startListener = true;

    private readonly Stat _stat;
    private readonly double _amount;
    private readonly StatModifierType _mode;

    public NightStatModify(EffectParameterSet parameters)
    {
        _stat = parameters.GetEnum<Stat>(XmlSkillEffectParameterType.Stat);
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF);

        // Run only once per daytime change.
        if (_startListener)
        {
            _startListener = false;

            // Init a global day-night change listener.
            GlobalEvents.Global.Subscribe<OnDayNightChange>(this, OnDayNightChange);
        }
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        _nightStatCharacters.Add(effected);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        _nightStatCharacters.Remove(effected);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        // Not night.
        if (!GameTimeTaskManager.getInstance().isNight())
            return;

        // Apply stat.
        switch (_mode)
        {
            case StatModifierType.DIFF:
            {
                effected.getStat().mergeAdd(_stat, _amount);
                break;
            }
            case StatModifierType.PER:
            {
                effected.getStat().mergeMul(_stat, _amount / 100 + 1);
                break;
            }
        }
    }

    private static void OnDayNightChange(OnDayNightChange @event)
    {
        // System message for Shadow Sense.
        SystemMessagePacket msg = new SystemMessagePacket(@event.isNight()
            ? SystemMessageId.IT_IS_NOW_MIDNIGHT_AND_THE_EFFECT_OF_S1_CAN_BE_FELT
            : SystemMessageId.IT_IS_DAWN_AND_THE_EFFECT_OF_S1_WILL_NOW_DISAPPEAR);

        msg.Params.addSkillName(_shadowSense);

        foreach (Creature creature in _nightStatCharacters)
        {
            // Pump again.
            creature.getStat().recalculateStats(true);

            // Send Shadow Sense message when player has skill.
            if (creature.getKnownSkill(_shadowSense) != null)
            {
                creature.sendPacket(msg);
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _amount, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._stat, x._amount, x._mode));
}