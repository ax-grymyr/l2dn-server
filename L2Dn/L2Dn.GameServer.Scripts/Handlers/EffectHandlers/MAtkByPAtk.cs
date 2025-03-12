using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MAtkByPAtk(StatSet @params): AbstractStatPercentEffect(@params, Stat.MAGIC_ATTACK_BY_PHYSICAL_ATTACK);