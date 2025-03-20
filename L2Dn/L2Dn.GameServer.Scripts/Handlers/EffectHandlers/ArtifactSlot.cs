using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ArtifactSlot(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.ARTIFACT_SLOTS);