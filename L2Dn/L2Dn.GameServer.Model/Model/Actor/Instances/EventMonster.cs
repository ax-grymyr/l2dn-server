using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class EventMonster: Monster
{
    // Block offensive skills usage on event mobs
    // mainly for AoE skills, disallow kill many event mobs
    // with one skill
    public bool block_skill_attack = false;

    // Event mobs should drop items to ground
    // but item pickup must be protected to killer
    // Todo: Some mobs need protect drop for spawner
    public bool drop_on_ground = false;

    public EventMonster(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.EventMob;
    }

    public void eventSetBlockOffensiveSkills(bool value)
    {
        block_skill_attack = value;
    }

    public void eventSetDropOnGround(bool value)
    {
        drop_on_ground = value;
    }

    public bool eventDropOnGround()
    {
        return drop_on_ground;
    }

    public bool eventSkillAttackBlocked()
    {
        return block_skill_attack;
    }
}