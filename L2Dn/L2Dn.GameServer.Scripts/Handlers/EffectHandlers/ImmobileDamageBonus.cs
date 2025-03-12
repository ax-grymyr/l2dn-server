using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Bonus damage to immobile targets.
/// </summary>
public sealed class ImmobileDamageBonus(StatSet @params): AbstractStatPercentEffect(@params, Stat.IMMOBILE_DAMAGE_BONUS);