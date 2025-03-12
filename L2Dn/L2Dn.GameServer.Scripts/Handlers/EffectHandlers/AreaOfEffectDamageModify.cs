using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AreaOfEffectDamageModify(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.AREA_OF_EFFECT_DAMAGE_MODIFY);