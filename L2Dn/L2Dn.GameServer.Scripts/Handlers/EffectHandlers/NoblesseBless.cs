using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Noblesse Blessing effect implementation.
 * @author earendil
 */
public class NoblesseBless: AbstractEffect
{
	public NoblesseBless(StatSet @params)
	{
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effector != null && effected != null && effected.isPlayable();
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.NOBLESS_BLESSING.getMask();
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.NOBLESSE_BLESSING;
	}
}