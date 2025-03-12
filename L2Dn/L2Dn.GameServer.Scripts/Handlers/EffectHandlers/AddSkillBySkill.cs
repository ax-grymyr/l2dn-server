using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AddSkillBySkill: AbstractEffect
{
    private readonly int _existingSkillId;
    private readonly int _existingSkillLevel;
    private readonly SkillHolder _addedSkill;

    public AddSkillBySkill(StatSet @params)
    {
        _existingSkillId = @params.getInt("existingSkillId");
        _existingSkillLevel = @params.getInt("existingSkillLevel");
        _addedSkill = new SkillHolder(@params.getInt("addedSkillId"), @params.getInt("addedSkillLevel"));
    }

    public override bool canPump(Creature? effector, Creature effected, Skill? skill)
    {
        return effected.isPlayer() && !effected.isTransformed() &&
            effected.getSkillLevel(_existingSkillId) == _existingSkillLevel;
    }

    public override void pump(Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        player.addSkill(_addedSkill.getSkill(), false);
        Utilities.ThreadPool.schedule(() =>
        {
            player.sendSkillList();
            player.getStat().recalculateStats(false);
            player.broadcastUserInfo();
        }, 100);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        effected.removeSkill(_addedSkill.getSkill(), false);
        Utilities.ThreadPool.schedule(() =>
        {
            player.sendSkillList();
            player.getStat().recalculateStats(false);
            player.broadcastUserInfo();
        }, 100);
    }

    public override int GetHashCode() => HashCode.Combine(_existingSkillId, _existingSkillLevel, _addedSkill);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._existingSkillId, x._existingSkillLevel, x._addedSkill));
}