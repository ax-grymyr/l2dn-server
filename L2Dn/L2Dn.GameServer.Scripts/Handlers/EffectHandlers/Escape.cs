using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Escape effect implementation.
 * @author Adry_85
 */
public class Escape: AbstractEffect
{
    private readonly TeleportWhereType? _escapeType;

    public Escape(StatSet @params)
    {
        if (@params.Contains("escapeType"))
            _escapeType = @params.getEnum<TeleportWhereType>("escapeType");
    }

    public override EffectType getEffectType() => EffectType.TELEPORT;

    public override bool isInstant() => true;

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        // While affected by escape blocking effect you cannot use Blink or Scroll of Escape
        return base.canStart(effector, effected, skill) && !effected.cannotEscape();
    }

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_escapeType != null)
        {
            Player? effectedPlayer = effected.getActingPlayer();
            TimedHuntingZoneHolder? huntingZone = effectedPlayer?.getTimedHuntingZone();
            if (effected.isInInstance() && effectedPlayer != null && effectedPlayer.isInTimedHuntingZone() &&
                huntingZone != null)
            {
                effected.teleToLocation(new Location(huntingZone.getEnterLocation(), 0),
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