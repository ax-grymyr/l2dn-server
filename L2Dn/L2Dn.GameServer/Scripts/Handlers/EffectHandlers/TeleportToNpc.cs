using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Teleport player/party to summoned npc effect implementation.
 * @author Nik
 */
public class TeleportToNpc: AbstractEffect
{
	private readonly int _npcId;
	private readonly bool _party;
	
	public TeleportToNpc(StatSet @params)
	{
		_npcId = @params.getInt("npcId");
		_party = @params.getBoolean("party", false);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.TELEPORT_TO_TARGET;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		ILocational teleLocation = null;
		foreach (Npc npc in effector.getSummonedNpcs())
		{
			if (npc.getId() == _npcId)
			{
				teleLocation = npc;
			}
		}
		
		if (teleLocation != null)
		{
			Party party = effected.getParty();
			if (_party && (party != null))
			{
				foreach (Player member in party.getMembers())
				{
					teleport(member, teleLocation);
				}
			}
			else
			{
				teleport(effected, teleLocation);
			}
		}
	}
	
	private void teleport(Creature effected, ILocational location)
	{
		if (effected.isInsideRadius2D(location, 900))
		{
			effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
			effected.broadcastPacket(new FlyToLocationPacket(effected, location, FlyType.DUMMY));
			effected.abortAttack();
			effected.abortCast();
			effected.setXYZ(location);
			effected.broadcastPacket(new ValidateLocationPacket(effected));
			effected.revalidateZone(true);
		}
		else
		{
			effected.teleToLocation(location);
		}
	}
}