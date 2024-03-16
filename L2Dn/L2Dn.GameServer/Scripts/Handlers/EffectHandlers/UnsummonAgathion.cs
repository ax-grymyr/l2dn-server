using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Unsummon Agathion effect implementation.
 * @author Zoey76
 */
public class UnsummonAgathion: AbstractEffect
{
	public UnsummonAgathion(StatSet @params)
	{
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effector.getActingPlayer();
		if (player != null)
		{
			int agathionId = player.getAgathionId();
			if (agathionId > 0)
			{
				player.setAgathionId(0);
				player.sendPacket(new ExUserInfoCubicPacket(player));
				player.broadcastCharInfo();
				
				if (player.Events.HasSubscribers<OnPlayerUnsummonAgathion>())
				{
					player.Events.NotifyAsync(new OnPlayerUnsummonAgathion(player, agathionId));
				}
			}
		}
	}
}