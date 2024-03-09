using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Mute effect implementation.
 */
public class Mute: AbstractEffect
{
	public Mute(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.MUTED.getMask();
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.MUTE;
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected == null) || effected.isRaid())
		{
			return;
		}
		
		effected.abortCast();
		effected.getAI().notifyEvent(CtrlEvent.EVT_MUTED);
	}
}