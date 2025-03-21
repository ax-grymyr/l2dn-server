using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("FeedModify")]
public sealed class FeedModify(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.FEED_MODIFY);