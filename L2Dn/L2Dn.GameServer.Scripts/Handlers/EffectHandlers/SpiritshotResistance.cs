using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SpiritshotResistance(StatSet @params): AbstractStatPercentEffect(@params, Stat.SPIRITSHOT_RESISTANCE);