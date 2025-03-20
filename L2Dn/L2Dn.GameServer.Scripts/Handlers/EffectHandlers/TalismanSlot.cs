using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Talisman Slot effect implementation.
/// </summary>
[AbstractEffectName("TalismanSlot")]
public sealed class TalismanSlot: AbstractEffect
{
    private readonly int _slots;

    public TalismanSlot(EffectParameterSet parameters)
    {
        _slots = parameters.GetInt32(XmlSkillEffectParameterType.Slots, 0);
    }

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effector != null && effected != null && effected.isPlayer();
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getActingPlayer()?.getStat().addTalismanSlots(_slots);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getActingPlayer()?.getStat().addTalismanSlots(-_slots);
    }

    public override int GetHashCode() => _slots;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._slots);
}