using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Summon Agathion effect implementation.
 * @author Zoey76
 */
public class SummonAgathion: AbstractEffect
{
	private readonly int _npcId;
	
	public SummonAgathion(StatSet @params)
	{
		if (@params.isEmpty())
		{
			LOGGER.Warn(GetType().Name + ": must have parameters.");
		}
		
		_npcId = @params.getInt("npcId", 0);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		Player player = effected.getActingPlayer();
		player.setAgathionId(_npcId);
		player.sendPacket(new ExUserInfoCubicPacket(player));
		player.broadcastCharInfo();
		
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_SUMMON_AGATHION))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerSummonAgathion(player, _npcId));
		}
	}
}