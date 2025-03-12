using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class HealEffect(StatSet @params): AbstractStatEffect(@params, Stat.HEAL_EFFECT, Stat.HEAL_EFFECT_ADD);