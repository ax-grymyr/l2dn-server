using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ResistDDMagic(StatSet @params): AbstractStatPercentEffect(@params, Stat.MAGIC_SUCCESS_RES);