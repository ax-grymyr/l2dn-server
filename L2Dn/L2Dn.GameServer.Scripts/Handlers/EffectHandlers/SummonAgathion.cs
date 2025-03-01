using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

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

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
        Player? player = effected.getActingPlayer();
		if (!effected.isPlayer() || player == null)
		{
			return;
		}

		player.setAgathionId(_npcId);
		player.sendPacket(new ExUserInfoCubicPacket(player));
		player.broadcastCharInfo();

		if (player.Events.HasSubscribers<OnPlayerSummonAgathion>())
		{
			player.Events.NotifyAsync(new OnPlayerSummonAgathion(player, _npcId));
		}
	}
}