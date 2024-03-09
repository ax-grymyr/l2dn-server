using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalShieldAngleAll: AbstractEffect
{
	public PhysicalShieldAngleAll(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.PHYSICAL_SHIELD_ANGLE_ALL.getMask();
	}
}