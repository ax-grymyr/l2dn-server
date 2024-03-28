using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AddSkillBySkill: AbstractEffect
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
	
	public override bool canPump(Creature effector, Creature effected, Skill skill)
	{
		return effected.isPlayer() && !effected.isTransformed() && (effected.getSkillLevel(_existingSkillId) == _existingSkillLevel);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getActingPlayer().addSkill(_addedSkill.getSkill(), false);
		Utilities.ThreadPool.schedule(() =>
		{
			effected.getActingPlayer().sendSkillList();
			effected.getActingPlayer().getStat().recalculateStats(false);
			effected.getActingPlayer().broadcastUserInfo();
		}, 100);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeSkill(_addedSkill.getSkill(), false);
		Utilities.ThreadPool.schedule(() =>
		{
			effected.getActingPlayer().sendSkillList();
			effected.getActingPlayer().getStat().recalculateStats(false);
			effected.getActingPlayer().broadcastUserInfo();
		}, 100);
	}
}