using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class HpCpHealCritical: AbstractEffect
{
	public HpCpHealCritical(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.HPCPHEAL_CRITICAL.getMask();
	}
}