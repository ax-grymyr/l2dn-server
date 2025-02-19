using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Refuel Airship effect implementation.
 * @author Adry_85
 */
public class RefuelAirship: AbstractEffect
{
	private readonly int _value;

	public RefuelAirship(StatSet @params)
	{
		_value = @params.getInt("value", 0);
	}

	public override EffectType getEffectType()
	{
		return EffectType.REFUEL_AIRSHIP;
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		AirShip? ship = effector.getActingPlayer()?.getAirShip();
        if (ship != null)
        {
            ship.setFuel(ship.getFuel() + _value);
            ship.updateAbnormalVisualEffects();
        }
    }
}