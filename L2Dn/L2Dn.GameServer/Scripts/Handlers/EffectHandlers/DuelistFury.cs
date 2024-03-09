using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author negaa
 */
public class DuelistFury: AbstractEffect
{
	public DuelistFury(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.DUELIST_FURY.getMask();
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effected.isPlayer();
	}
}