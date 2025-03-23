using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("CanRefuelAirship")]
public sealed class CanRefuelAirshipSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public CanRefuelAirshipSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
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

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}