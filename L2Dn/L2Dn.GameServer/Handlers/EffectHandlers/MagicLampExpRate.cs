using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets.MagicLamp;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius, Serenitty
 */
public class MagicLampExpRate: AbstractStatPercentEffect
{
	public MagicLampExpRate(StatSet @params): base(@params, Stat.MAGIC_LAMP_EXP_RATE)
	{
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(Stat.MAGIC_LAMP_EXP_RATE, _amount);
		if ((skill != null))
		{
			effected.getStat().mergeAdd(Stat.LAMP_BONUS_EXP, _amount);
			effected.getStat().mergeAdd(Stat.LAMP_BONUS_BUFFS_COUNT, 1d);
		}
		
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		player.sendPacket(new ExMagicLampInfoPacket(player));
	}
}