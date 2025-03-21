using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Item Effect: Increase/decrease PK count permanently.
/// </summary>
[HandlerStringKey("PkCount")]
public sealed class PkCount: AbstractEffect
{
    private readonly int _amount;

    public PkCount(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        if (player.getPkKills() > 0)
        {
            int newPkCount = Math.Max(player.getPkKills() + _amount, 0);
            player.setPkKills(newPkCount);
            player.updateUserInfo();
        }
    }

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}