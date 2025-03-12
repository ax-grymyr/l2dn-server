using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Resist damage while immobile.
/// </summary>
public sealed class ImmobileDamageResist(StatSet @params): AbstractStatPercentEffect(@params, Stat.IMMOBILE_DAMAGE_RESIST);