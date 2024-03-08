using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author hlwrave
 */
public class ArtifactSlot: AbstractStatAddEffect
{
	public ArtifactSlot(StatSet @params): base(@params, Stat.ARTIFACT_SLOTS)
	{
	}
}