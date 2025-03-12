using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class RealDamageResist(StatSet @params): AbstractStatPercentEffect(@params, Stat.REAL_DAMAGE_RESIST);