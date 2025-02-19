using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpInstantzoneSkillCondition: ISkillCondition
{
    private readonly int _instanceId;

    public OpInstantzoneSkillCondition(StatSet @params)
    {
        _instanceId = @params.getInt("instanceId");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Instance? instance = caster.getInstanceWorld();
        return instance != null && instance.getTemplateId() == _instanceId;
    }
}