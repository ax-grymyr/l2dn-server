using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class VampiricDefence(StatSet @params): AbstractStatPercentEffect(@params, Stat.ABSORB_DAMAGE_DEFENCE);