using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Geremy
 */
public class FeedModify: AbstractStatEffect
{
	public FeedModify(StatSet @params): base(@params, Stat.FEED_MODIFY)
	{
	}
}