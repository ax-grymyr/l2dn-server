using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Root effect implementation.
 * @author mkizub
 */
public class Root: AbstractEffect
{
	public Root(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.ROOTED.getMask();
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.ROOT;
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (!effected.isPlayer())
		{
			effected.getAI().notifyEvent(CtrlEvent.EVT_THINK);
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected == null) || effected.isRaid())
		{
			return;
		}
		
		effected.stopMove(null);
		effected.getAI().notifyEvent(CtrlEvent.EVT_ROOTED);
	}
}