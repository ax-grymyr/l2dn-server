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
 * Give Clan reputation effect implementation.
 * @author Mobius
 */
public class GiveClanReputation: AbstractEffect
{
	private readonly int _reputation;
	
	public GiveClanReputation(StatSet @params)
	{
		_reputation = @params.getInt("reputation", 0);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effector.isPlayer() || !effected.isPlayer() || effected.isAlikeDead() || (effector.getActingPlayer().getClan() == null))
		{
			return;
		}
		
		effector.getActingPlayer().getClan().addReputationScore(_reputation);
		
		foreach (ClanMember member in effector.getActingPlayer().getClan().getMembers())
		{
			if (member.isOnline())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CLAN_REPUTATION_POINTS_S1);
				sm.Params.addInt(_reputation);
				member.getPlayer().sendPacket(sm);
			}
		}
	}
}