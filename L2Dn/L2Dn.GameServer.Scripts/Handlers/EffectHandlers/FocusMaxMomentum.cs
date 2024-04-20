using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Focus Max Energy effect implementation.
 * @author Adry_85
 */
public class FocusMaxMomentum: AbstractEffect
{
	public FocusMaxMomentum(StatSet @params)
	{
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer())
		{
			Player player = effected.getActingPlayer();
			
			int count = (int) effected.getStat().getValue(Stat.MAX_MOMENTUM, 1);
			
			player.setCharges(count);
			
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_FORCE_HAS_INCREASED_TO_LEVEL_S1);
			sm.Params.addInt(count);
			player.sendPacket(sm);
			
			player.sendPacket(new EtcStatusUpdatePacket(player));
		}
	}
}