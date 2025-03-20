using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Effect that blocks damage and heals to HP/MP.
/// Regeneration or DOT shouldn't be blocked, Vampiric Rage and Balance Life as well.
/// </summary>
[HandlerName("DamageBlock")]
public sealed class DamageBlock: AbstractEffect
{
    private readonly bool _blockHp;
    private readonly bool _blockMp;

    public DamageBlock(EffectParameterSet parameters)
    {
        string type = parameters.GetString(XmlSkillEffectParameterType.Type, string.Empty);
        _blockHp = type.equalsIgnoreCase("BLOCK_HP");
        _blockMp = type.equalsIgnoreCase("BLOCK_MP");
    }

    public override EffectFlags EffectFlags => _blockHp ? EffectFlags.HP_BLOCK : _blockMp ? EffectFlags.MP_BLOCK : EffectFlags.NONE;

    public override int GetHashCode() => HashCode.Combine(_blockHp, _blockMp);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._blockHp, x._blockMp));
}