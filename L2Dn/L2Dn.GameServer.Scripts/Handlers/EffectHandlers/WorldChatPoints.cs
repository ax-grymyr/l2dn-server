using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class WorldChatPoints(StatSet @params): AbstractStatEffect(@params, Stat.WORLD_CHAT_POINTS);