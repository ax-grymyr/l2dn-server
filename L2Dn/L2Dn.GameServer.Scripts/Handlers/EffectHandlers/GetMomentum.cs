using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw, Mobius
 */
public class GetMomentum: AbstractEffect
{
	private static int _ticks;
	
	public GetMomentum(StatSet @params)
	{
		_ticks = @params.getInt("ticks", 0);
	}
	
	public override int getTicks()
	{
		return _ticks;
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer())
		{
			Player player = effected.getActingPlayer();
			int maxCharge = (int) player.getStat().getValue(Stat.MAX_MOMENTUM, 1);
			int newCharge = Math.Min(player.getCharges() + 1, maxCharge);
			
			player.setCharges(newCharge);
			
			if (newCharge == maxCharge)
			{
				player.sendPacket(SystemMessageId.YOUR_FORCE_HAS_REACHED_MAXIMUM_CAPACITY);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_FORCE_HAS_INCREASED_TO_LEVEL_S1);
				sm.Params.addInt(newCharge);
				player.sendPacket(sm);
			}
			
			player.sendPacket(new EtcStatusUpdatePacket(player));
		}
		
		return skill.isToggle();
	}
}