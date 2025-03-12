using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalAttackRange(StatSet @params): AbstractStatEffect(@params, Stat.PHYSICAL_ATTACK_RANGE);