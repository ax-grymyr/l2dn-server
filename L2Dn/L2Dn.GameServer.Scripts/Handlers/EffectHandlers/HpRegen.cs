using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class HpRegen(StatSet @params): AbstractConditionalHpEffect(@params, Stat.REGENERATE_HP_RATE);