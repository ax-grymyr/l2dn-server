using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class CallRandomSkill: AbstractEffect
{
	private readonly List<SkillHolder> _skills = new();
	
	public CallRandomSkill(StatSet @params)
	{
		String skills = @params.getString("skills", null);
		if (skills != null)
		{
			foreach (string skill in skills.Split(";"))
			{
				_skills.add(new SkillHolder(int.Parse(skill.Split(",")[0]), int.Parse(skill.Split(",")[1])));
			}
		}
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		SkillCaster.triggerCast(effector, effected, _skills.get(Rnd.get(_skills.size())).getSkill());
	}
}