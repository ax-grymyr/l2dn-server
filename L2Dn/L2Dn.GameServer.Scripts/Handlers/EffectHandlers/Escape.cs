using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Escape effect implementation.
/// </summary>
[HandlerStringKey("Escape")]
public sealed class Escape: AbstractEffect
{
    private readonly TeleportWhereType? _escapeType;

    public Escape(EffectParameterSet parameters)
    {
        _escapeType = parameters.GetEnumOptional<TeleportWhereType>(XmlSkillEffectParameterType.EscapeType);
    }

    public override EffectTypes EffectTypes => EffectTypes.TELEPORT;

    public override bool IsInstant => true;

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        // While affected by escape blocking effect you cannot use Blink or Scroll of Escape
        return base.CanStart(effector, effected, skill) && !effected.cannotEscape();
    }

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_escapeType != null)
        {
            Player? effectedPlayer = effected.getActingPlayer();
            TimedHuntingZoneHolder? huntingZone = effectedPlayer?.getTimedHuntingZone();
            if (effected.isInInstance() && effectedPlayer != null && effectedPlayer.isInTimedHuntingZone() &&
                huntingZone != null)
            {
                effected.teleToLocation(new Location(huntingZone.EnterLocation, 0),
                    effected.getInstanceId());
            }
            else
            {
                effected.teleToLocation(_escapeType.Value, null);
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_escapeType);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._escapeType);
}