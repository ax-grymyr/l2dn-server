using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class CallRandomSkill: AbstractEffect
{
	private readonly List<SkillHolder> _skills = [];

	public CallRandomSkill(StatSet @params)
	{
		string skills = @params.getString("skills", string.Empty);
		if (!string.IsNullOrEmpty(skills))
		{
			foreach (string skill in skills.Split(";"))
			{
				_skills.Add(new SkillHolder(int.Parse(skill.Split(",")[0]), int.Parse(skill.Split(",")[1])));
			}
		}
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		SkillCaster.triggerCast(effector, effected, _skills.GetRandomElement().getSkill());
	}
}