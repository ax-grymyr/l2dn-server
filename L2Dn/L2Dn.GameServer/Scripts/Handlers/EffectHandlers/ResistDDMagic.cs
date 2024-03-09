using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ResistDDMagic: AbstractStatPercentEffect
{
	public ResistDDMagic(StatSet @params): base(@params, Stat.MAGIC_SUCCESS_RES)
	{
	}
}