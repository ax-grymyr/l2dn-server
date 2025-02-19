using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid, Mobius
 */
public class OpExistNpcSkillCondition: ISkillCondition
{
    private readonly List<int> _npcIds;
    private readonly int _range;
    private readonly bool _isAround;

    public OpExistNpcSkillCondition(StatSet @params)
    {
        _npcIds = @params.getList<int>("npcIds");
        _range = @params.getInt("range");
        _isAround = @params.getBoolean("isAround");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        foreach (Npc npc in World.getInstance().getVisibleObjectsInRange<Npc>(caster, _range))
        {
            if (_npcIds.Contains(npc.getId()))
                return _isAround;
        }

        return !_isAround;
    }
}