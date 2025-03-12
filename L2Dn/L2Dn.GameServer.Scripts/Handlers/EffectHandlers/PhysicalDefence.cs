using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalDefence(StatSet @params): AbstractConditionalHpEffect(@params, Stat.PHYSICAL_DEFENCE);