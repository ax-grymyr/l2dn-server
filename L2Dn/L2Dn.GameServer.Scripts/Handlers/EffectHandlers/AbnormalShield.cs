using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * An effect that blocks a debuff. Acts like DOTA's Linken Sphere.
 * @author Nik
 */
public class AbnormalShield: AbstractEffect
{
	private readonly int _times;

	public AbnormalShield(StatSet @params)
	{
		_times = @params.getInt("times", -1);
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.setAbnormalShieldBlocks(_times);
	}

	public override long getEffectFlags()
	{
		return EffectFlag.ABNORMAL_SHIELD.getMask();
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.setAbnormalShieldBlocks(int.MinValue);
	}

	public override EffectType getEffectType()
	{
		return EffectType.ABNORMAL_SHIELD;
	}
}