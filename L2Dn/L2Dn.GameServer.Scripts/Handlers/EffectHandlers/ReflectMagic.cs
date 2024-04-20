using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ReflectMagic: AbstractStatAddEffect
{
	public ReflectMagic(StatSet @params): base(@params, Stat.VENGEANCE_SKILL_MAGIC_DAMAGE)
	{
	}
}