using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class CanRefuelAirshipSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public CanRefuelAirshipSkillCondition(StatSet @params)
    {
        _amount = @params.getInt("amount");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        bool canRefuelAirship = true;
        Player? player = caster.getActingPlayer();
        AirShip? airShip = player?.getAirShip();
        if (airShip is not ControllableAirShip || airShip.getFuel() + _amount > airShip.getMaxFuel())
        {
            canRefuelAirship = false;
        }

        return canRefuelAirship;
    }
}