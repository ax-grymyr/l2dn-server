using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

// This class is here mostly for convenience and for avoidance of hardcoded IDs.
// It refers to Beast (mobs) that can be attacked but can also be fed
// For example, the Beast Farm's Alpen Buffalo.
// This class is only truly used by the handlers in order to check the correctness
// of the target.  However, no additional tasks are needed, since they are all
// handled by scripted AI.
public class FeedableBeast : Monster
{
    public FeedableBeast(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.FeedableBeast);
    }
}
