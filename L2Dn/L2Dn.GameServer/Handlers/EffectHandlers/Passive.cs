using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Passive effect implementation.
 * @author Mobius
 */
public class Passive: AbstractEffect
{
	public Passive(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.PASSIVE.getMask();
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effected.isAttackable();
	}
}