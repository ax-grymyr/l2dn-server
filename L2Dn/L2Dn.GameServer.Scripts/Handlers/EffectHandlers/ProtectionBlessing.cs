using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Protection Blessing effect implementation.
 * @author kerberos_20
 */
public class ProtectionBlessing: AbstractEffect
{
	public ProtectionBlessing(StatSet @params)
	{
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effector != null && effected != null && effected.isPlayer();
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.PROTECTION_BLESSING.getMask();
	}
}