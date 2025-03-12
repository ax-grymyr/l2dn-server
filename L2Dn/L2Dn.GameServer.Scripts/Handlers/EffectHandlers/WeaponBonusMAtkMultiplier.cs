using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class WeaponBonusMAtkMultiplier(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER);