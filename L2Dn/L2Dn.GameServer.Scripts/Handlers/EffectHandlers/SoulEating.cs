using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Playables;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Soul Eating effect implementation.
 * @author UnAfraid
 */
public class SoulEating: AbstractEffect
{
	private readonly SoulType _type;
	private readonly int _expNeeded;
	private readonly Double _maxSouls;
	
	public SoulEating(StatSet @params)
	{
		_type = @params.getEnum("type", SoulType.LIGHT);
		_expNeeded = @params.getInt("expNeeded");
		_maxSouls = @params.getDouble("maxSouls");
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer())
		{
			Player player = (Player)effected;
			player.Events.Subscribe<OnPlayableExpChanged>(this, onExperienceReceived);
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (effected.isPlayer())
		{
			Player player = (Player)effected;
			player.Events.Unsubscribe<OnPlayableExpChanged>(onExperienceReceived);
		}
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(Stat.MAX_SOULS, _maxSouls);
	}
	
	private void onExperienceReceived(OnPlayableExpChanged ev)
	{
		Playable playable = ev.getPlayable();
		long exp = ev.getNewExp() - ev.getOldExp();
		
		// TODO: Verify logic.
		if (playable.isPlayer() && (exp >= _expNeeded))
		{
			Player player = playable.getActingPlayer();
			int maxSouls = (int) player.getStat().getValue(Stat.MAX_SOULS, 0);
			if (player.getChargedSouls(_type) >= maxSouls)
			{
				playable.sendPacket(SystemMessageId.YOU_CAN_T_ABSORB_MORE_SOULS);
				return;
			}
			
			player.increaseSouls(1, _type);
			
			if ((player.getTarget() != null) && player.getTarget().isNpc())
			{
				Npc npc = (Npc) playable.getTarget();
				player.broadcastPacket(new ExSpawnEmitterPacket(player, npc), 500);
			}
		}
	}
}