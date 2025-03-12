using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SoulshotResistance(StatSet @params): AbstractStatPercentEffect(@params, Stat.SOULSHOT_RESISTANCE);