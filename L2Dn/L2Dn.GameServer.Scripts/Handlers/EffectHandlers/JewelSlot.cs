using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class JewelSlot(StatSet @params): AbstractStatAddEffect(@params, Stat.BROOCH_JEWELS);