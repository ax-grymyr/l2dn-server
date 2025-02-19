using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Physical Attack Mute effect implementation.
 * @author -Rnn-
 */
public class PhysicalAttackMute: AbstractEffect
{
	public PhysicalAttackMute(StatSet @params)
	{
	}

	public override long getEffectFlags()
	{
		return EffectFlag.PSYCHICAL_ATTACK_MUTED.getMask();
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.startPhysicalAttackMuted();
	}
}