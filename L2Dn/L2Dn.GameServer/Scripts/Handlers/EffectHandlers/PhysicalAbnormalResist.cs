using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalAbnormalResist: AbstractStatAddEffect
{
	public PhysicalAbnormalResist(StatSet @params): base(@params, Stat.ABNORMAL_RESIST_PHYSICAL)
	{
	}
}