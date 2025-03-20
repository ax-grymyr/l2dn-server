using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Handlers;

public interface IAbstractEffect
{
    EffectTypes EffectTypes { get; }
}