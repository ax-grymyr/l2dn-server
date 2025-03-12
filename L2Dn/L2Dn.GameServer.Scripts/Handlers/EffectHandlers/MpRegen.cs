using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MpRegen(StatSet @params): AbstractStatEffect(@params, Stat.REGENERATE_MP_RATE);