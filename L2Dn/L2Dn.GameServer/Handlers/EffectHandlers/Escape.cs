using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Escape effect implementation.
 * @author Adry_85
 */
public class Escape: AbstractEffect
{
	private readonly TeleportWhereType? _escapeType;
	
	public Escape(StatSet @params)
	{
		if (@params.contains("escapeType"))
			_escapeType = @params.getEnum<TeleportWhereType>("escapeType");
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.TELEPORT;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		// While affected by escape blocking effect you cannot use Blink or Scroll of Escape
		return base.canStart(effector, effected, skill) && !effected.cannotEscape();
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_escapeType != null)
		{
			if (effected.isInInstance() && effected.getActingPlayer().isInTimedHuntingZone())
			{
				effected.teleToLocation(effected.getActingPlayer().getTimedHuntingZone().getEnterLocation(), effected.getInstanceId());
			}
			else
			{
				effected.teleToLocation(_escapeType.Value, null);
			}
		}
	}
}