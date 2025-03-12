using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SpiritExpModify(StatSet @params): AbstractStatEffect(@params, Stat.ELEMENTAL_SPIRIT_BONUS_EXP);