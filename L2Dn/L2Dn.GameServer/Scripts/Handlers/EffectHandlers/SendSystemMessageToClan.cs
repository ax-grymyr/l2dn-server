using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Item Effect: Increase/decrease PK count permanently.
 * @author Nik
 */
public class SendSystemMessageToClan: AbstractEffect
{
	private readonly SystemMessagePacket _message;
	
	public SendSystemMessageToClan(StatSet @params)
	{
		int id = @params.getInt("id", 0);
		_message = new SystemMessagePacket((SystemMessageId)id);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effected.getActingPlayer();
		if ((player == null))
		{
			return;
		}
		
		Clan clan = player.getClan();
		if (clan != null)
		{
			clan.broadcastToOnlineMembers(_message);
		}
	}
}