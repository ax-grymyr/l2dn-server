using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Set Skill effect implementation.
 * @author Zoey76
 */
public class SetSkill: AbstractEffect
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	
	public SetSkill(StatSet @params)
	{
		_skillId = @params.getInt("skillId", 0);
		_skillLevel = @params.getInt("skillLevel", 1);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		Skill setSkill = SkillData.getInstance().getSkill(_skillId, _skillLevel);
		if (setSkill == null)
		{
			return;
		}
		
		effected.getActingPlayer().addSkill(setSkill, true);
	}
}