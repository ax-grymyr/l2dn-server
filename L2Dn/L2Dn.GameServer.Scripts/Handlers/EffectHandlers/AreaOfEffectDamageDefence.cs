using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AreaOfEffectDamageDefence(StatSet @params)
    : AbstractStatEffect(@params, Stat.AREA_OF_EFFECT_DAMAGE_DEFENCE);