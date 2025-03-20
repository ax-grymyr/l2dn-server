using L2Dn.Collections;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class TargetRaceSkillCondition: ISkillCondition
{
    private readonly EnumSet64<Race> _races;

    public TargetRaceSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string> races = parameters.GetStringList(XmlSkillConditionParameterType.Race);
        foreach (string race in races)
            _races.Add(Enum.Parse<Race>(race, true));
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isCreature())
            return false;

        return _races.Contains(((Creature)target).getRace());
    }
}