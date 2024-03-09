using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Physical Mute effect implementation.
 * @author -Nemesiss-
 */
public class PhysicalMute: AbstractEffect
{
	public PhysicalMute(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.PSYCHICAL_MUTED.getMask();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.getAI().notifyEvent(CtrlEvent.EVT_MUTED);
	}
}