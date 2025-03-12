using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalAttackSpeed(StatSet @params): AbstractStatEffect(@params, Stat.PHYSICAL_ATTACK_SPEED);