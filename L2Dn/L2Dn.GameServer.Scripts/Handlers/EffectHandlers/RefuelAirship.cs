using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Refuel Airship effect implementation.
/// </summary>
[AbstractEffectName("RefuelAirship")]
public sealed class RefuelAirship: AbstractEffect
{
    private readonly int _value;

    public RefuelAirship(EffectParameterSet parameters)
    {
        _value = parameters.GetInt32(XmlSkillEffectParameterType.Value, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.REFUEL_AIRSHIP;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        AirShip? ship = effector.getActingPlayer()?.getAirShip();
        if (ship != null)
        {
            ship.setFuel(ship.getFuel() + _value);
            ship.updateAbnormalVisualEffects();
        }
    }

    public override int GetHashCode() => _value;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._value);
}