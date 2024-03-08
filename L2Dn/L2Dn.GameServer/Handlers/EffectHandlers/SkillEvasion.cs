using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Note: In retail this effect doesn't stack. It appears that the active value is taken from the last such effect.
 * @author Sdw
 */
public class SkillEvasion: AbstractEffect
{
	private readonly int _magicType;
	private readonly double _amount;
	
	public SkillEvasion(StatSet @params)
	{
		_magicType = @params.getInt("magicType", 0);
		_amount = @params.getDouble("amount", 0);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.getStat().addSkillEvasionTypeValue(_magicType, _amount);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getStat().removeSkillEvasionTypeValue(_magicType, _amount);
	}
}