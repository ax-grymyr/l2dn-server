using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class WeaponBonusPAtkMultiplier(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER);