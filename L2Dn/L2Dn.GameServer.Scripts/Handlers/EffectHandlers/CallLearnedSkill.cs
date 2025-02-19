using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Call Learned Skill by Level effect implementation.
 * @author Kazumi
 */
public class CallLearnedSkill: AbstractEffect
{
	private readonly int _skillId;

	public CallLearnedSkill(StatSet @params)
	{
		_skillId = @params.getInt("skillId");
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Skill? knownSkill = effector.getKnownSkill(_skillId);
		if (knownSkill != null)
		{
			SkillCaster.triggerCast(effector, effected, knownSkill);
		}
	}
}