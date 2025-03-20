using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("ResistDDMagic")]
public sealed class ResistDDMagic(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.MAGIC_SUCCESS_RES);