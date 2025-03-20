using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("WorldChatPoints")]
public sealed class WorldChatPoints(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.WORLD_CHAT_POINTS);