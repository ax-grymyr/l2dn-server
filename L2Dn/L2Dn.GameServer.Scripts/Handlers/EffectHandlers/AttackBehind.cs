using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Nik
 */
public class AttackBehind: AbstractEffect
{
	public AttackBehind(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.ATTACK_BEHIND.getMask();
	}
}