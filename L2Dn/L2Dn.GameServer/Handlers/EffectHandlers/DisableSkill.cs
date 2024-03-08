using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Ofelin
 */
public class DisableSkill: AbstractEffect
{
	private readonly Set<int> _disabledSkills;
	
	public DisableSkill(StatSet @params)
	{
		String disable = @params.getString("disable");
		if ((disable != null) && !disable.isEmpty())
		{
			_disabledSkills = new();
			foreach (String slot in disable.Split(";"))
			{
				_disabledSkills.add(int.Parse(slot));
			}
		}
		else
		{
			_disabledSkills = new();
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		foreach (int disableSkillId in _disabledSkills)
		{
			Skill? knownSkill = effected.getKnownSkill(disableSkillId);
			if (knownSkill != null)
			{
				effected.disableSkill(knownSkill, TimeSpan.Zero);
			}
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		foreach (int enableSkillId in _disabledSkills)
		{
			Skill? knownSkill = effected.getKnownSkill(enableSkillId);
			if (knownSkill != null)
			{
				if (effected.isPlayer())
				{
					effected.getActingPlayer().enableSkill(knownSkill, false);
				}
				else
				{
					effected.enableSkill(knownSkill);
				}
			}
		}
	}
}