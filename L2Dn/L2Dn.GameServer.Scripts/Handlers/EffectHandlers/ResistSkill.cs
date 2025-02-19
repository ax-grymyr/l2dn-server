using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Resist Skill effect implementaion.
 * @author UnAfraid
 */
public class ResistSkill: AbstractEffect
{
	private readonly List<SkillHolder> _skills = [];

	public ResistSkill(StatSet @params)
	{
		for (int i = 1;; i++)
		{
			int skillId = @params.getInt("skillId" + i, 0);
			int skillLevel = @params.getInt("skillLevel" + i, 0);
			if (skillId == 0)
			{
				break;
			}
			_skills.Add(new SkillHolder(skillId, skillLevel));
		}

		if (_skills.Count == 0)
		{
			throw new ArgumentException(GetType().Name + ": Without parameters!");
		}
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		foreach (SkillHolder holder in _skills)
		{
			effected.addIgnoreSkillEffects(holder);
		}
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		foreach (SkillHolder holder in _skills)
		{
			effected.removeIgnoreSkillEffects(holder);
		}
	}
}