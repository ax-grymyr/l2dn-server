using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalAbnormalResist(StatSet @params): AbstractStatAddEffect(@params, Stat.ABNORMAL_RESIST_PHYSICAL);