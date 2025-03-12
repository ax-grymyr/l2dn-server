using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class RearDamage(StatSet @params): AbstractStatPercentEffect(@params, Stat.REAR_DAMAGE_RATE);