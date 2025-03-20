using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class CpRegen(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.REGENERATE_CP_RATE);